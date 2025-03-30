local util = require("core.util.init")
local CoreActions = require("core.xstate.actions.core_actions")

local StateMachine = {
    monoBehaviourCSharp = nil,
    config = nil,
    currentState = nil,
    states = {}
}

-- Prepare action functions for each action configuration.
local function prepareActions(actionConfigs, actions)
    local actionFns = {}

    local function processAction(actionConfig)
        local actionFn = CoreActions[actionConfig.type] or actions[actionConfig.type]
        if not actionFn then
            error("Action not found: " .. actionConfig.type)
        end

        table.insert(actionFns, function(stateMachine)
            actionFn(stateMachine, actionConfig.params)
        end)
    end

    if util.tableUtil.isList(actionConfigs) then
        for _, actionConfig in ipairs(actionConfigs) do
            processAction(actionConfig)
        end
    else
        processAction(actionConfigs)
    end

    return actionFns
end

-- Prepare invoke functions for each invoke configuration.
local function prepareInvokes(invokeConfigs, actions)
    local actionFns = {}

    local function processInvoke(invokeConfig)
        local actionFn = CoreActions[invokeConfig.src] or actions[invokeConfig.src]
        if not actionFn then
            error("Action not found: " .. invokeConfig.src)
        end

        table.insert(actionFns, function(stateMachine)
            actionFn(stateMachine, invokeConfig.input)
        end)
    end

    if util.tableUtil.isList(invokeConfigs) then
        for _, invokeConfig in ipairs(invokeConfigs) do
            processInvoke(invokeConfig)
        end
    else
        processInvoke(invokeConfigs)
    end

    return actionFns
end

-- Set the current state and trigger the entry/exit functions.
function StateMachine:setState(name)
    if name == self.currentState then
        -- TODO: Sau này có thể thêm self transition (new event nhưng lại quay về state cũ)
        return
    end

    if self.currentState and self.states[self.currentState] then
        local exitState = self.states[self.currentState]
        if exitState.exit then
            exitState:exit(self)
        end
    end

    self.currentState = name

    if self.states[name] and self.states[name].entry then
        self.states[name]:entry(self)
    end
end

-- Get the current state name.
function StateMachine:getCurrentState()
    return self.currentState
end

-- Build the states and define actions, transitions, etc.
function StateMachine:new(config, actions)
    local instance = {
        _type = "StateMachine",
        config = config,
        states = {},
        currentState = nil
    }

    -- Process states' entry, invoke, and exit functions.
    for name, stateConfig in pairs(config.states) do
        local entryFns = stateConfig.entry and prepareActions(stateConfig.entry, actions) or {}
        local invokeFns = stateConfig.invoke and prepareInvokes(stateConfig.invoke, actions) or {}
        local exitFns = stateConfig.exit and prepareActions(stateConfig.exit, actions) or {}

        local state = {}

        function state:entry(stateMachine)
            for _, fn in ipairs(entryFns) do
                fn(stateMachine)
            end
        end

        function state:invoke(stateMachine)
            for _, fn in ipairs(invokeFns) do
                fn(stateMachine)
            end
        end

        function state:exit(stateMachine)
            for _, fn in ipairs(exitFns) do
                fn(stateMachine)
            end
        end

        function state:dispatch(stateMachine, event, params)
            local onEvent = stateConfig.on and stateConfig.on[event]
            if onEvent then
                local nextState = onEvent[1] -- hardcoded index 1, see xstate config
                if nextState and nextState.target then
                    -- TODO: need to support call actions here
                    stateMachine:setState(nextState.target)
                else
                    error("Invalid transition for event: " .. event)
                end
            end
        end

        instance.states[name] = state
    end

    -- Method này sẽ expose cho C# nên sẽ không có self
    function instance.LuaStateMachineMono()
        return {
            Start = function(csharpObject)
                instance:setState(instance.config.initial)
            end,

            Update = function(csharpObject)
                instance.states[instance.currentState]:invoke(instance)
            end,

            OnDestroy = function(csharpObject)
                if instance.currentState and instance.states[instance.currentState] then
                    instance.states[instance.currentState]:exit(instance)
                end
            end,

            Dispatch = function(csharpObject, event, params)
                if instance.currentState and instance.states[instance.currentState] then
                    instance.states[instance.currentState]:dispatch(instance, event, params)
                else
                    error("Cannot dispatch event: no current state")
                end
            end,

            Set = function(csharpObject, key, value)
                instance[key] = value
            end,

            Initialize = function(csharpObject)
                print("Initialize StateMachine")
            end
        }
    end

    self.__index = self
    setmetatable(instance, self)

    return instance
end

return StateMachine
