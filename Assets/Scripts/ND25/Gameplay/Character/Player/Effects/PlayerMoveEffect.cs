using ND25.Core.XMachine;
using ND25.Util.Common.Enum;
using R3;
using UnityEngine;
namespace ND25.Gameplay.Character.Player.Effects
{
    public class PlayerMoveEffect : XMachineEffect<PlayerContext>
    {

        public PlayerMoveEffect(PlayerActor actor) : base(actor: actor)
        {
        }

        [XMachineEffect]
        public XMachineActionHandler MoveHandler()
        {
            return upstream => upstream.OfAction(xAction: PlayerAction.MoveAction)
                .Select(selector: action =>
                {
                    PlayerActor playerActor = (PlayerActor)actor;
                    Vector2 moveInput = playerActor.inputActions.Player.Move.ReadValue<Vector2>();
                    playerActor.machine.SetContext(contextUpdater: playerContext =>
                    {
                        playerContext.xInputDirection = Direction.ConvertToXDirection(velocity: moveInput.x);
                    });
                    playerActor.SetVelocity(moveInput: moveInput);

                    if (action.GetBool(key: PlayerAction.DataKey.Transtion))
                    {
                        playerActor.machine.Transition(toStateId: moveInput.x != 0 ? PlayerState.Move : PlayerState.Idle);
                    }

                    return XMachineAction.Empty;
                });
        }
    }
}
