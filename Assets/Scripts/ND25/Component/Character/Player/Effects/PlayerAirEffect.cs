using ND25.Core.XMachine;
using R3;
using System;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerAirEffect : XMachineEffect<PlayerContext>
    {

        public PlayerAirEffect(XMachineActor<PlayerContext> actor) : base(actor: actor)
        {
        }

        [XMachineEffect]
        public XMachineActionHandler CheckFallGround()
        {
            return action => action.OfAction(xAction: PlayerAction.CheckFallGroundAction)
                .ThrottleLast(timeSpan: TimeSpan.FromMilliseconds(value: 100))
                .Select(selector: _ =>
                {
                    if (actor.objectChecker.isGrounded)
                    {
                        actor.machine.Transition(toStateId: PlayerState.Idle);
                    }

                    return XMachineAction.Empty;
                });
        }

        [XMachineEffect]
        public XMachineActionHandler CheckNotInGroundAction()
        {
            return action => action.OfAction(xAction: PlayerAction.CheckNotInGroundAction)
                .Select(selector: _ =>
                {
                    if (!actor.objectChecker.isGrounded)
                    {
                        actor.machine.Transition(toStateId: PlayerState.Air);
                    }

                    return XMachineAction.Empty;
                });
        }
    }
}
