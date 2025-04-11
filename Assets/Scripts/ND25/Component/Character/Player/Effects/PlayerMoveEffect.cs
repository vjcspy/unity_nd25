using ND25.Core.XMachine;
using ND25.Util.Common.Enum;
using R3;
using UnityEngine;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerMoveEffect : XMachineEffect<PlayerContext>
    {

        public PlayerMoveEffect(PlayerActor actor) : base(actor: actor)
        {
        }

        [XMachineEffect]
        public XMachineActionHandler MoveHandler()
        {
            return upstream => upstream.OfAction(xAction: PlayerAction.XInputListenAction)
                .Select(selector: _ =>
                {
                    PlayerActor playerActor = (PlayerActor)actor;
                    Vector2 moveInput = playerActor.pcControls.GamePlay.Move.ReadValue<Vector2>();
                    playerActor.machine.SetContext(contextUpdater: playerContext =>
                    {
                        playerContext.xInput = Direction.ConvertToXDirection(moveInput.x);
                        return playerContext;
                    });
                    playerActor.SetVelocity(moveInput: moveInput);
                    return XMachineAction.Empty;
                });
        }
    }
}
