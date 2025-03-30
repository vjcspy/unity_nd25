local TableUtil = {} -- Tạo bảng module

function TableUtil.isList(tbl)
    for key, _ in pairs(tbl) do
        if type(key) ~= "number" then
            return false
        end
    end
    return true
end

return TableUtil -- Trả về bảng chứa các hàm tiện ích
