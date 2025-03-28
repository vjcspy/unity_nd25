print("Loaded Player script")

local M = {}  -- tạo module table

local rb

function M.start()
    rb = M.self:GetComponent(typeof(CS.UnityEngine.Rigidbody2D))
end

function M.update()
    if rb:IsTouchingLayers(CS.UnityEngine.LayerMask.GetMask("Ground")) then
        local randomForce = math.random(1, 20) / 10
        local force = CS.UnityEngine.Vector2(0, randomForce)
        rb:AddForce(force, CS.UnityEngine.ForceMode2D.Impulse)
    end
end

function M.ondestroy()
    print("lua destroy")
end

return M 