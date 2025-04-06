using Cysharp.Threading.Tasks;
using ND25.Core.ReactiveMachine;
using R3;
using System;
using UnityEngine;
namespace ND25.Character.Warrior.Actor
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
                    warriorReactiveMachine.warriorAnimator.UpdateParam(WarriorAnimator.Param.yVelocity, warriorContext.yVelocity);
                    if (!warriorReactiveMachine.groundChecker.isGrounded)
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
                .OfAction(WarriorAction.XInputChangeTransition)
                .Select(
                    _ => ReactiveMachineCoreAction.TransitionActionFactory.Create(xInput != 0 ? WarriorEvent.run : WarriorEvent.idle)
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler ForceStopRun()
        {
            return upstream => upstream
                .OfAction(WarriorAction.ForceStopRun)
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
                .OfAction(WarriorAction.XInputHandler)
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

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler UpdateAnimatorParams()
        {
            return upstream => upstream
                .OfAction(WarriorAction.UpdateAnimatorParams)
                .Select(
                    action =>
                    {

                        if (action.payload == null)
                        {
                            return ReactiveMachineCoreAction.Empty;
                        }

                        foreach ((string keyString, object value) in action.payload)
                        {
                            // Debug.Log("Update animator key: " + keyString);
                            switch (value)
                            {
                                case bool boolVal:
                                    warriorReactiveMachine.warriorAnimator.UpdateParam(keyString, boolVal);
                                    break;
                                case float floatVal:
                                    warriorReactiveMachine.warriorAnimator.UpdateParam(keyString, floatVal);
                                    break;
                                case double doubleVal:
                                    warriorReactiveMachine.warriorAnimator.UpdateParam(keyString, (float)doubleVal);
                                    break;
                                default:
                                    Debug.Log("Unsupported type: " + value.GetType());
                                    break;
                            }
                        }

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }
    }
}
