print("Loaded Player script");

local M = {
    id = 1,
    monoBehaviour = nil
}

function M:start()
    print("lua start with script id: " .. self.id)
end

function M:update()
    self.monoBehaviour:TryJumpIfGrounded()
end

function M:destroy()
    print("lua destroy")
end

function M:new()
    local newObj = {
        id = math.random(1, 1000),
        monoBehaviour = nil
    }

    function newObj.Start()
        self.start(newObj)
    end

    function newObj.Update()
        self.update(newObj)
    end

    function newObj.Destroy()
        self.destroy(newObj)
    end

    self.__index = self
    setmetatable(newObj, self)
    return newObj
end

return M
