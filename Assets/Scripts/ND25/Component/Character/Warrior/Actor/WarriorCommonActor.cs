using ND25.Core.ReactiveMachine;
using R3;
using System;
using UnityEngine;
namespace ND25.Component.Character.Warrior.Actor
{

    public class WarriorCommonActor : WarriorActorBase
    {

        float xInput;
        public WarriorCommonActor(WarriorReactiveMachine warriorReactiveMachine) : base(warriorReactiveMachine)
        {
            HandleContextChange();
        }

        void Flip()
        {
            warriorReactiveMachine.transform.localScale = warriorReactiveMachine.rb.linearVelocity.x switch
            {
                > 0 => new Vector3(1, 1, 1),
                < 0 => new Vector3(-1, 1, 1),
                _ => warriorReactiveMachine.transform.localScale
            };
        }

        void HandleContextChange()
        {
            warriorReactiveMachine.machine.ContextChangeHandler(context => context
                .ThrottleLast(TimeSpan.FromMilliseconds(150))
                .Select(warriorContext =>
                {
                    warriorReactiveMachine.animatorParam.UpdateParam(WarriorAnimatorParamName.yVelocity, warriorContext.yVelocity);
                    if (!warriorReactiveMachine.ObjectChecker.isGrounded)
                    {
                        warriorReactiveMachine.machine.DispatchEvent(WarriorEvent.air);
                    }

                    return Unit.Default;
                }));
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler XInputChangeTransition()
        {
            return upstream => upstream
                .OfAction(WarriorActionType.XInputChangeTransition)
                .Select(
                    _ => ReactiveMachineCoreAction.TransitionActionFactory.Create(xInput != 0 ? WarriorEvent.run : WarriorEvent.idle)
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler ForceStopRun()
        {
            return upstream => upstream
                .OfAction(WarriorActionType.ForceStopRun)
                .Select(
                    _ =>
                    {
                        float yVelocity = warriorReactiveMachine.rb.linearVelocity.y;
                        Vector2 newVelocity = new Vector2(0, yVelocity);
                        warriorReactiveMachine.rb.linearVelocity = newVelocity;

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler XInputHandler()
        {
            return upstream => upstream
                .OfAction(WarriorActionType.XInputHandler)
                .Select(
                    _ =>
                    {
                        xInput = Input.GetAxis("Horizontal");
                        float yVelocity = warriorReactiveMachine.rb.linearVelocity.y;
                        Vector2 newVelocity = new Vector2(xInput * warriorReactiveMachine.moveSpeed, yVelocity);
                        warriorReactiveMachine.rb.linearVelocity = newVelocity;
                        Flip();

                        warriorReactiveMachine.machine.SetContext(
                            context =>
                            {
                                context.yVelocity = yVelocity;

                                return context;
                            }
                        );

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }
    }
}
