using ND25.Core.XMachine;
using R3;
using System.Collections.Generic;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerAirEffect : XMachineEffect<PlayerContext>
    {


        public PlayerAirEffect(XMachineActor<PlayerContext> actor) : base(actor: actor)
        {
        }
        private static IEnumerable<XMachineAction> GetJumpSequenceActions()
        {
            yield return PlayerAction.ForceJumpAction;
            yield return XMachineAction.Transition.Factory(data: PlayerState.Air);
        }

        [XMachineEffect]
        public XMachineActionHandler JumpHandler()
        {
            return action =>
            {
                return action.OfAction(xAction: PlayerAction.JumpHandlerAction).SelectMany(selector: _ =>
                {
                    PlayerActor playerActor = (PlayerActor)actor;

                    if (!playerActor.pcControls.GamePlay.Jump.triggered || !playerActor.objectChecker.isGrounded)
                    {
                        return Observable.Return(value: XMachineAction.Empty);
                    }

                    return GetJumpSequenceActions().ToObservable();
                });
            };
        }
    }
}
