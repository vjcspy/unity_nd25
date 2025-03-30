local isList = function(tbl)
    for key, _ in pairs(tbl) do
        if type(key) ~= "number" then
            return false
        end
    end
    return true
end

local printTable = function(tbl)
    local function tableToString(t)
        local result = "{"
        local first = true
        for k, v in pairs(t) do
            if not first then
                result = result .. ", "
            else
                first = false
            end
            local keyStr = ""
            if type(k) == "string" then
                keyStr = k .. "="
            else
                keyStr = "[" .. tostring(k) .. "]="
            end

            local valueStr = ""
            if type(v) == "table" then
                valueStr = tableToString(v) -- Đệ quy với bảng con
            else
                valueStr = tostring(v)
            end
            result = result .. keyStr .. valueStr
        end
        result = result .. "}"
        return result
    end

    print(tableToString(tbl))
end

return {
    isList = isList,
    printTable = printTable
}
