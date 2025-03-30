-- Import thư viện dkjson (đảm bảo đã cài đặt dkjson)
local json = require("dkjson")

-- Đọc file JSON
local file = io.open("data.json", "r") -- Mở file data.json để đọc
if not file then
    print("Không thể mở file data.json.")
    return
end

local content = file:read("*a") -- Đọc toàn bộ nội dung file
file:close() -- Đóng file

-- Phân tích JSON thành Lua table
local luaTable, pos, err = json.decode(content, 1, nil)
if err then
    print("Lỗi khi phân tích JSON: " .. err)
    return
end

-- Mở file output.lua để ghi kết quả
local outfile = io.open("output.lua", "w")
if not outfile then
    print("Không thể mở file output.lua để ghi.")
    return
end

-- Hàm ghi table ra file dưới định dạng giống như khi print
local function writeTable(f, t, indent)
    indent = indent or "" -- Nếu không có indent, mặc định là chuỗi rỗng
    for k, v in pairs(t) do
        if type(v) == "table" then
            f:write(indent .. "[" .. (type(k) == "string" and '"' .. k .. '"' or k) .. "] = {\n")
            writeTable(f, v, indent .. "  ")
            f:write(indent .. "},\n")
        else
            local valueStr
            if type(v) == "string" then
                valueStr = '"' .. v .. '"'
            elseif type(v) == "boolean" then
                valueStr = tostring(v)
            else
                valueStr = tostring(v)
            end
            f:write(indent .. "[" .. (type(k) == "string" and '"' .. k .. '"' or k) .. "] = " .. valueStr .. ",\n")
        end
    end
end

outfile:write("return {\n")
writeTable(outfile, luaTable, "  ")
outfile:write("}\n")

outfile:close()

print("Đã lưu table vào file output.lua")