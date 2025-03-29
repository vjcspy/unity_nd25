local warror_state_config = require("character.xstate.warrior.config")
local XState = require("core.xstate.xstate")


xstate = XState:build(warror_state_config, {
    updateAnimator = updateAnimator,
})

function xstate:updateAnimator()
    print("updateAnimator")
    for key, value in pairs(params) do
        xstate:updateAnimator(key, value)
    end
end