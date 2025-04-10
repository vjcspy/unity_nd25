using ND25.Core.XMachine;
using R3;
using UnityEngine;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerAirEffect : XMachineEffect<PlayerContext>
    {

        public PlayerAirEffect(XMachineActor<PlayerContext> actor) : base(actor: actor)
        {
        }

        [XMachineEffect]
        public XMachineActionHandler JumpHandler()
        {
            return action =>
            {
                return action.OfAction(xAction: PlayerAction.JumpHandlerAction).Select(selector: _ =>
                {
                    PlayerActor playerActor = (PlayerActor)actor;
                    if (!playerActor.pcControls.GamePlay.Jump.triggered || !playerActor.objectChecker.isGrounded)
                    {
                        return XMachineAction.Empty;
                    }

                    playerActor.machine.SetContext(
                        context =>
                        {
                            context.lastJumpTime = Time.time;

                            return context;
                        }
                    );
                    playerActor.ForceJump();
                    playerActor.machine.Transition(PlayerState.Air);

                    return XMachineAction.Empty;
                });
            };
        }
    }
}
