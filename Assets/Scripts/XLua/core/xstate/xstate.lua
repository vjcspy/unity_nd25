local util = require("core.util.init")

local XState = {
    monoBehaviour = nil,
    config = nil,
    currentState = nil
}

local coreLogAction = require("core.xstate.actions.core_actions").coreLogAction
local coreActions = {
    coreLogAction = coreLogAction
}

function XState:setState(name)
    if self.currentState and self.states[self.currentState].exit then
        self.states[self.currentState].exit()
    end
    self.currentState = name
    if self.states[name].entry then
        self.states[name].entry()
    end
end

function XState:getCurrentState()
    return self.currentState
end

local function preapreAction(actionConfig, actions)
    local actionFns = {}
    local function processAction(action)
        local actionName = action.type
        local actionParams = action.params

        if coreActions[actionName] then
            -- Nếu action là một hàm trong coreActions, gọi hàm đó với các tham số đã cho
            actionFns[#actionFns + 1] = function()
                coreActions[actionName](actionParams)
            end
        elseif actions[actionName] then
            -- Nếu action là một hàm trong actions, gọi hàm đó với các tham số đã cho
            actionFns[#actionFns + 1] = function()
                actions[actionName](actionParams)
            end
        else
            -- Nếu không tìm thấy action, có thể ném lỗi hoặc xử lý theo cách khác
            error("Action not found: " .. actionName)
        end
    end

    if util.table_ulti.is_list(actionConfig) then
        for _, action in ipairs(actionConfig) do
            processAction(action)
        end
    else
        processAction(actionConfig)
    end

    return actionFns
end

function XState:new()
    local newObj = {
    }
    self.__index = self
    return newObj
end

-- Thêm các hàm vào bảng XState
function XState:build(config, actions)
    states = {}

    for name, state in pairs(config.states) do
        entryFn = state.entry and preapreAction(state.entry, actions) or {}
        invokeFn = state.invoke and preapreAction(state.invoke, actions) or {}
        exitFn = state.exit and preapreAction(state.exit, actions) or {}

        states[name] = {
            entry = function()
                for _, fn in ipairs(entryFn) do
                    fn()
                end
            end,
            invoke = function()
                for _, fn in ipairs(invokeFn) do
                    fn()
                end
            end,
            exit = function()
                for _, fn in ipairs(exitFn) do
                    fn()
                end
            end,
            dispatch = function(event, params)
                if config.on[event] then
                    -- TODO: cân handle thêm actions, guard, meta trong này
                    nextState = config.on[event][0] -- hardcode index 0
                    self:setState(nextState.target)
                else
                    -- Nếu không có sự kiện nào được định nghĩa, có thể ném lỗi hoặc xử lý theo cách khác
                    error("Dispatch a unknown event: " .. event)
                end
            end,
            
        }

        if state.on then
            for event, actions in pairs(state.on) do
                states[name][event] = function()
                    for _, action in ipairs(actions) do
                        local targetState = action.target
                        if states[targetState] then
                            self:setState(targetState)
                        end
                    end
                end
            end
        end
    end

    local newObj = {
        currentState = config.initial,
        config = config,
        states = states
    }

    function newObj.LuaMonoXState()
        return {
            Start = function()
                self.states[self.currentState].entry()
            end,
            Update = function()
                self.states[self.currentState].invoke()
            end,
            OnDestroy = function()
                self.states[self.currentState].exit()
            end,
            Dispatch = function(event, params)
                self.states[self.currentState].dispatch(event, params)
            end
        }
    end

    self.__index = self
    setmetatable(newObj, self)

    return newObj
end


return XState
