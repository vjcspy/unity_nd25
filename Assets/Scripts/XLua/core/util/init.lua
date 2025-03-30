local tableUtil = require("core.util.table_ulti")
local Configuration = require("core.configuration")

local log = function(...)
    if Configuration.DEBUG then
        local args = {...}
        local result = {"log"}
        local typeHandlers = {
            string = tostring,
            number = tostring,
            boolean = tostring,
            table = function(tbl)
                local str = "{"
                for key, value in pairs(tbl) do
                    str = str .. (type(key) == "string" and key or tostring(key)) .. "=" .. tostring(value) .. ", "
                end
                return str:sub(1, -3) .. "}" -- Remove the last comma and space
            end
        }

        for _, v in ipairs(args) do
            local handler = typeHandlers[type(v)] or tostring
            table.insert(result, handler(v))
        end

        print(table.concat(result))
    end
end

return {
    log = log,
    tableUtil = tableUtil,
}
