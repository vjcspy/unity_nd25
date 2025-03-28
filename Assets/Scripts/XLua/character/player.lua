print("Loaded Player script");

local M = {}

function M:start()
    print("lua start");
end

function M:update()
    M.monoBehaviour:TryJumpIfGrounded()  -- Gọi trực tiếp `self`, không cần `M.self`
end

function M:ondestroy()
    print("lua destroy");
end

function M:new()
    local newObj = {}
    self.__index = self
    setmetatable(newObj, self)
    return newObj
end

return M
