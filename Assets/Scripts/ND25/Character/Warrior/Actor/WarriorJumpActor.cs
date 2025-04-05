using Cysharp.Threading.Tasks;
using ND25.Core.ReactiveMachine;
using R3;
using System;
using UnityEngine;
namespace ND25.Character.Warrior.Actor
{
    public class WarriorJumpActor : WarriorActorBase
    {
        public WarriorJumpActor(WarriorMonoBehavior warriorMonoBehavior) : base(warriorMonoBehavior)
        {
        }
        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler WhenFallGround()
        {
            return upstream => upstream
                .OfAction(WarriorAction.WhenFallGround)
                .Where(_ => warriorMonoBehavior.machine.context.Value.lastJumpTime < Time.time - 0.2f)
                .ThrottleLast(TimeSpan.FromMilliseconds(200))
                .Select(
                    _ =>
                    {
                        UniTask.Post(() =>
                        {
                            if (warriorMonoBehavior.groundChecker.isGrounded)
                            {
                                warriorMonoBehavior.machine.DispatchEvent(WarriorEvent.idle);
                            }
                        });
                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler ForceJump()
        {
            return upstream => upstream
                .OfAction(WarriorAction.ForceJump)
                .Select(
                    _ =>
                    {
                        if (!warriorMonoBehavior.groundChecker.isGrounded)
                        {
                            return ReactiveMachineCoreAction.Empty;
                        }
                        Vector2 jumpForceVector = Vector2.up * warriorMonoBehavior.jumpForce;
                        warriorMonoBehavior.rb.AddForce(jumpForceVector, ForceMode2D.Impulse);

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler WhenYInputChange()
        {
            return upstream => upstream
                .OfAction(WarriorAction.WhenYInputChange)
                .Select(
                    _ =>
                    {
                        if (!Input.GetKeyDown(KeyCode.Space) || !warriorMonoBehavior.groundChecker.isGrounded)
                        {
                            return ReactiveMachineCoreAction.Empty;
                        }

                        warriorMonoBehavior.machine.DispatchEvent(WarriorEvent.jump);
                        warriorMonoBehavior.machine.SetContext(
                            context =>
                            {
                                context.lastJumpTime = Time.time;

                                return context;
                            }
                        );

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }
    }
}
