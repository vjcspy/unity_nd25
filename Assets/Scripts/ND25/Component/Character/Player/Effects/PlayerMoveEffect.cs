using ND25.Core.XMachine;
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
            return upstream => upstream
                .OfAction(actions: new[]
                {
                    PlayerAction.MoveHandler
                })
                .Select(
                    selector: _ =>
                    {
                        PlayerActor playerActor = (PlayerActor)actor;
                        Vector2 xInput = playerActor.pcControls.GamePlay.Move.ReadValue<Vector2>();
                        Vector2 newVelocity = new Vector2(x: xInput.x * playerActor.moveSpeed, y: playerActor.rb.linearVelocity.y);
                        playerActor.rb.linearVelocity = newVelocity;
                        return XMachineAction.Empty;
                    }
                );
        }
    }
}
