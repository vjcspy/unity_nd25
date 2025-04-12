using ND25.Core.XMachine;
using R3;
using System;
using UnityEngine;
namespace ND25.Gameplay.Character.Player.Effects
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

        [XMachineEffect]
        public XMachineActionHandler ForceJump()
        {
            return upstream => upstream.OfAction(xAction: PlayerAction.ForceJump)
                .Select(selector: action =>
                {
                    PlayerActor playerActor = (PlayerActor)actor;
                    float jumpForce = action.GetFloat(key: PlayerAction.DataKey.JumpForce);
                    if (jumpForce == 0)
                    {
                        playerActor.ForceJump();
                    }
                    else
                    {
                        Vector2 jumpForceVector = Vector2.up * jumpForce;
                        actor.rb.AddForce(force: jumpForceVector, mode: ForceMode2D.Impulse);
                        action.Clear();
                    }

                    return XMachineAction.Empty;
                });
        }
    }
}
