local warror_state_config = require("character.xstate.warrior.warrior_state_config")
local StateMachine = require("core.xstate.state_machine")

TableUtil = {}

function TableUtil:factory()
    return StateMachine:new(warror_state_config, {
        handleMove = function(stateMachine, input)
            stateMachine.monoBehaviourCSharp:HandleMove()
        end
    })
end

return TableUtil
