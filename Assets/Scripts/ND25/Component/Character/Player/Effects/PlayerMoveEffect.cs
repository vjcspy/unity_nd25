using ND25.Core.XMachine;
using UnityEngine;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerMoveEffect : XMachineEffect<PlayerContext>
    {

        public PlayerMoveEffect(PlayerActor actor) : base(actor: actor)
        {
        }

        [XMachineSubscribe(PlayerAction.MOVE_HANDLER)]
        public void MoveHandler(XMachineAction _)
        {
            PlayerActor playerActor = (PlayerActor)actor;
            Vector2 moveInput = playerActor.pcControls.GamePlay.Move.ReadValue<Vector2>();
            playerActor.SetVelocity(moveInput: moveInput);
            playerActor.machine.SetContext(contextUpdater: playerContext =>
            {
                playerContext.xInput = moveInput.x;
                return playerContext;
            });
        }
    }
}
