local M = {}

function M:new()
    local newObj = {}
    setmetatable(newObj, self)
    self.__index = self
    return newObj
end

local b = M:new()
b.test(1)
b.test(b,2)