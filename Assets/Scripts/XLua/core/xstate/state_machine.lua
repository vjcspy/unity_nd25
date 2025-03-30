local Ulti = require("core.util.init")
local CoreActions = require("core.xstate.actions.core_actions")

local coreActions = {
    coreLogAction = CoreActions.coreLogAction,
    coreUpdateAnimator = CoreActions.coreUpdateAnimator
}
local StateMachine = {
    monoBehaviourCSharp = nil,
    config = nil,
    currentState = nil
}

-- Set the current state and trigger the entry/exit functions.
function StateMachine:setState(name)
    if StateMachine.currentState and StateMachine.states[StateMachine.currentState].exit then
        StateMachine.states[StateMachine.currentState].exit()
    end

    StateMachine.currentState = name

    if StateMachine.states[name].entry then
        StateMachine.states[name].entry()
    end
end

-- Get the current state name.
function StateMachine:getCurrentState()
    return StateMachine.currentState
end

-- Prepare action functions for each action configuration.
local function prepareActions(actionConfigs, actions)
    local actionFns = {}

    local function processAction(actionConfig)
        local actionFn = coreActions[actionConfig.type] or actions[actionConfig.type]
        if not actionFn then
            error("Action not found: " .. actionConfig.type)
        end

        table.insert(actionFns, function(stateMachine)
            actionFn(stateMachine, actionConfig.params)
        end)
    end

    if Ulti.TableUtil.isList(actionConfigs) then
        for _, actionConfig in ipairs(actionConfigs) do
            processAction(actionConfig)
        end
    else
        processAction(actionConfigs)
    end

    return actionFns
end

local function prepareInvokes(invokeConfigs, actions)
    local actionFns = {}

    local function processInvoke(actionConfig)
        local actionFn = coreActions[actionConfig.src] or actions[actionConfig.src]
        if not actionFn then
            error("Action not found: " .. actionConfig.src)
        end

        table.insert(actionFns, function(stateMachine)
            actionFn(stateMachine, actionConfig.input)
        end)
    end

    if Ulti.TableUtil.isList(invokeConfigs) then
        for _, actionConfig in ipairs(invokeConfigs) do
            processInvoke(actionConfig)
        end
    else
        processInvoke(invokeConfigs)
    end

    return actionFns
end

-- Build the states and define actions, transitions, etc.
function StateMachine:new(config, actions)
    local states = {}

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
            local onEvent = stateConfig.on[event]
            if onEvent then
                local nextState = onEvent[0] -- hardcoded index 0
                StateMachine:setState(nextState.target)
            else
                error("Dispatch an unknown event: " .. event)
            end
        end

        states[name] = state
    end

    local newObj = {
        _type = "StateMachine",
        config = config,
        states = states
    }

    function newObj.LuaStateMachineMono()
        return {
            Start = function(csharpObject)
                newObj.states[newObj.currentState]:entry(newObj)
            end,
            Update = function(csharpObject)
                newObj.states[newObj.currentState]:invoke(newObj)
            end,
            OnDestroy = function(csharpObject)
                newObj.states[newObj.currentState]:exit(newObj)
            end,
            Dispatch = function(csharpObject, event, params)
                newObj.states[newObj.currentState]:dispatch(newObj, event, params)
            end,
            SetData = function(csharpObject, key, value)
                newObj[key] = value
            end,
            Initilize = function(csharpObject)
                newObj:setState(newObj.config.initial)
            end
        }
    end

    self.__index = self
    setmetatable(newObj, self)

    return newObj
end

return StateMachine
