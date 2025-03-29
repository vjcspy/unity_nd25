local M = {}  -- Tạo bảng module

function M.is_list(t)
    local i = 1
    for k, _ in pairs(t) do
        if k ~= i then
            return false  -- Nếu khóa không phải là số liên tiếp, không phải list
        end
        i = i + 1
    end
    return true  -- Nếu tất cả các khóa là số liên tiếp, là list
end

return M  -- Trả về bảng chứa các hàm tiện ích