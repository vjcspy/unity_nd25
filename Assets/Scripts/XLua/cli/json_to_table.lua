-- Import thư viện dkjson (đảm bảo đã cài đặt dkjson)
local json = require("dkjson")

-- Đọc file JSON
local file = io.open("data.json", "r") -- Mở file data.json để đọc
if not file then
    print("Không thể mở file.")
    return
end

local content = file:read("*a") -- Đọc toàn bộ nội dung file
file:close() -- Đóng file

-- Phân tích JSON thành Lua table
local luaTable, pos, err = json.decode(content, 1, nil)

-- Kiểm tra lỗi khi phân tích JSON
if err then
    print("Lỗi khi phân tích JSON: " .. err)
    return
end

function printTable(t, indent)
    indent = indent or "" -- Nếu không có indent, mặc định là chuỗi rỗng
    for k, v in pairs(t) do
        -- Nếu giá trị là bảng, gọi đệ quy để in bảng con
        if type(v) == "table" then
            print(indent .. "[" .. (type(k) == "string" and '"' .. k .. '"' or k) .. "] = {")
            printTable(v, indent .. "  ")
            print(indent .. "},")
        else
            -- Xử lý các kiểu dữ liệu khác nhau (chuỗi, boolean, số)
            local valueStr
            if type(v) == "string" then
                valueStr = '"' .. v .. '"'
            elseif type(v) == "boolean" then
                valueStr = tostring(v)  -- Chuyển boolean thành chuỗi "true" hoặc "false"
            else
                valueStr = tostring(v)  -- Chuyển các kiểu khác thành chuỗi
            end

            print(indent .. "[" .. (type(k) == "string" and '"' .. k .. '"' or k) .. "] = " .. valueStr .. ",")
        end
    end
end

print("return {")
printTable(luaTable, "  ")
print("}")
