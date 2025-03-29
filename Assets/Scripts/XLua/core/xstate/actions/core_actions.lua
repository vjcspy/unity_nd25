local M = {}

function M.coreLogAction(...)
    local args = {...}
    local result = {}

    local typeHandlers = {
        string = tostring,
        number = tostring,
        boolean = tostring,
        table = function(tbl)
            local str = "{"
            for key, value in pairs(tbl) do
                str = str .. (type(key) == "string" and key or tostring(key)) .. "=" .. tostring(value) .. ", "
            end
            return str:sub(1, -3) .. "}"  -- Loại bỏ dấu phẩy và khoảng trắng cuối cùng
        end
    }

    for _, v in ipairs(args) do
        local handler = typeHandlers[type(v)] or tostring
        table.insert(result, handler(v))
    end

    print(table.concat(result))
end

function M.updateAnimator(params)
    for key, value in pairs(params) do
        -- Giả sử bạn có một hàm updateAnimator trong một module nào đó
        -- Gọi hàm updateAnimator với các tham số đã cho
        -- updateAnimator(key, value)
        print("Updating animator: " .. key .. " to " .. tostring(value))
    end
end

return M
