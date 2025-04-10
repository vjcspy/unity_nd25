using ND25.Core.XMachine;
using R3;
using System;
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
            yield return new XMachineActionWithPayload<Enum>(type: "", payload: PlayerState.Air);
        }

        [XMachineEffect]
        public XMachineActionHandler JumpInputListenAction()
        {
            return action =>
            {
                return action.OfAction(xAction: PlayerAction.JumpInputListenAction).Select(selector: _ =>
                {
                    PlayerActor playerActor = (PlayerActor)actor;

                    if (!playerActor.pcControls.GamePlay.Jump.triggered || !playerActor.objectChecker.isGrounded)
                    {
                        return XMachineAction.Empty;
                    }

                    return PlayerAction.ForceJumpAction;
                });
            };
        }

        [XMachineEffect]
        public XMachineActionHandler CheckFallGround()
        {
            return action => action.OfAction(xAction: PlayerAction.CheckFallGroundAction)
                .ThrottleLast(timeSpan: TimeSpan.FromMilliseconds(value: 100))
                .Select(selector: _ =>
                {
                    PlayerActor playerActor = (PlayerActor)actor;
                    if (playerActor.objectChecker.isGrounded)
                    {
                        playerActor.machine.Transition(toStateId: PlayerState.Idle);
                    }

                    return XMachineAction.Empty;
                });
        }

        [XMachineEffect]
        public XMachineActionHandler CheckFalling()
        {
            return action => action.OfAction(xAction: PlayerAction.CheckNotInGroundAction)
                .ThrottleLast(timeSpan: TimeSpan.FromMilliseconds(value: 100))
                .Select(selector: _ =>
                {
                    PlayerActor playerActor = (PlayerActor)actor;
                    if (!playerActor.objectChecker.isGrounded)
                    {
                        playerActor.machine.Transition(toStateId: PlayerState.Air);
                    }

                    return XMachineAction.Empty;
                });
        }
    }
}
