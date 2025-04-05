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
        public WarriorCommonActor(WarriorMonoBehavior warriorMonoBehavior) : base(warriorMonoBehavior)
        {
            HandleContextChange();
        }

        void Flip()
        {
            warriorMonoBehavior.transform.localScale = warriorMonoBehavior.rb.linearVelocity.x switch
            {
                > 0 => new Vector3(1, 1, 1),
                < 0 => new Vector3(-1, 1, 1),
                _ => warriorMonoBehavior.transform.localScale
            };
        }

        void HandleContextChange()
        {
            warriorMonoBehavior.machine.ContextChangeHandler(context => context
                .ThrottleLast(TimeSpan.FromMilliseconds(200))
                .Select(warriorContext =>
                {
                    UniTask.Post(() =>
                    {
                        warriorMonoBehavior.warriorAnimator.UpdateParam(WarriorAnimator.Param.yVelocity, warriorContext.yVelocity);
                    });

                    return Unit.Default;
                }));
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler WhenXInputChange()
        {
            return upstream => upstream
                .OfAction(WarriorAction.WhenXInputChange)
                .Select(
                    _ =>
                    {
                        warriorMonoBehavior.machine.DispatchEvent(xInput != 0 ? WarriorEvent.run : WarriorEvent.idle);
                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler HandleXInput()
        {
            return upstream => upstream
                .OfAction(WarriorAction.HandleXInput)
                .Select(
                    _ =>
                    {
                        xInput = Input.GetAxis("Horizontal");
                        float yVelocity = warriorMonoBehavior.rb.linearVelocity.y;
                        Vector2 newVelocity = new Vector2(xInput * warriorMonoBehavior.moveSpeed, yVelocity);
                        warriorMonoBehavior.rb.linearVelocity = newVelocity;
                        Flip();

                        warriorMonoBehavior.machine.SetContext(
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
                                    warriorMonoBehavior.warriorAnimator.UpdateParam(keyString, boolVal);
                                    break;
                                case float floatVal:
                                    warriorMonoBehavior.warriorAnimator.UpdateParam(keyString, floatVal);
                                    break;
                                case double doubleVal:
                                    warriorMonoBehavior.warriorAnimator.UpdateParam(keyString, (float)doubleVal);
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
