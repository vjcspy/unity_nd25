using ND25.Core.XMachine;
using ND25.Util.Common.Enum;
using R3;
using UnityEngine;
namespace ND25.Gameplay.Character.Player.Effects
{
    public class PlayerMoveEffect : XMachineEffect<PlayerContext>
    {
        private readonly PlayerActor playerActor;
        public PlayerMoveEffect(PlayerActor actor) : base(actor: actor)
        {
            playerActor = actor;
        }

        [XMachineEffect]
        public XMachineActionHandler MoveHandler()
        {
            return upstream => upstream.OfAction(xAction: PlayerAction.MoveAction)
                .Select(selector: action =>
                {
                    Vector2 moveInput = playerActor.inputActions.Player.Move.ReadValue<Vector2>();
                    playerActor.machine.SetContext(contextUpdater: playerContext =>
                    {
                        XDirection xDirection = Direction.ConvertToXDirection(velocity: moveInput.x);
                        // playerContext.xInputDirection = xDirection;
                        if (xDirection == XDirection.None)
                        {
                            return;
                        }
                        playerContext.xFacingDirection = xDirection;
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
