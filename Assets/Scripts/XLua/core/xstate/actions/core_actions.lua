local M = {}
local ultil = require("core.util.init")
local Configuration = require("core.configuration")

local coreLogAction = function(stateMachine, ...)
    if Configuration.DEBUG ~= true then
        return
    end

    local args = {...}
    local result = {"coreLogAction"}

    local typeHandlers = {
        string = tostring,
        number = tostring,
        boolean = tostring,
        table = function(tbl)
            local str = "{"
            for key, value in pairs(tbl) do
                str = str .. (type(key) == "string" and key or tostring(key)) .. "=" .. tostring(value) .. ", "
            end
            return str:sub(1, -3) .. "}" -- Loại bỏ dấu phẩy và khoảng trắng cuối cùng
        end
    }

    for _, v in ipairs(args) do
        local handler = typeHandlers[type(v)] or tostring
        table.insert(result, handler(v))
    end

    print(table.concat(result))
end

local coreUpdateAnimator = function(stateMachine, params)
    ultil.log("coreUpdateAnimator", params)
    for key, value in pairs(params) do
        stateMachine.monoBehaviourCSharp:UpdateAnimator(key, value)
    end
end

local coreHandleMove = function(stateMachine)
    stateMachine.monoBehaviourCSharp:HandleUserInput()
end

return {
    coreLogAction = coreLogAction,
    coreUpdateAnimator = coreUpdateAnimator,
    coreHandleMove = coreHandleMove,
    -- ["monoBehaviourCSharp:HandleUserInput"] = coreHandleMove
}
