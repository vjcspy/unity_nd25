using ND25.Core.ReactiveMachine;
using R3;
using System;
using UnityEngine;
namespace ND25.Component.Character.Warrior.Actor
{
    public class WarriorAirActor : WarriorActorBase
    {
        public WarriorAirActor(WarriorReactiveMachine warriorReactiveMachine) : base(warriorReactiveMachine)
        {
        }
        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler FallGroundTransition()
        {
            return upstream => upstream
                .OfAction(WarriorActionType.FallGroundTransition)
                .ThrottleLast(TimeSpan.FromMilliseconds(200))
                .Where(_ => warriorReactiveMachine.machine.context.Value.lastJumpTime < Time.time - 0.2f)
                .Select(
                    _ => warriorReactiveMachine.groundChecker.isGrounded ? ReactiveMachineCoreAction.TransitionActionFactory.Create(WarriorEvent.idle) : ReactiveMachineCoreAction.Empty
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler ForceJump()
        {
            return upstream => upstream
                .OfAction(WarriorActionType.ForceJump)
                .Select(
                    _ =>
                    {
                        if (!warriorReactiveMachine.groundChecker.isGrounded)
                        {
                            return ReactiveMachineCoreAction.Empty;
                        }

                        Vector2 jumpForceVector = Vector2.up * warriorReactiveMachine.jumpForce;
                        warriorReactiveMachine.rb.AddForce(jumpForceVector, ForceMode2D.Impulse);

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler YInputChangeTransition()
        {
            return upstream => upstream
                .OfAction(WarriorActionType.YInputChangeTransition)
                .Select(
                    _ =>
                    {
                        if (!Input.GetKeyDown(KeyCode.Space) || !warriorReactiveMachine.groundChecker.isGrounded)
                        {
                            return ReactiveMachineCoreAction.Empty;
                        }

                        warriorReactiveMachine.machine.SetContext(
                            context =>
                            {
                                context.lastJumpTime = Time.time;

                                return context;
                            }
                        );

                        return ReactiveMachineAction.Create(WarriorActionType.ForceJump);
                    }
                );
        }
    }
}
