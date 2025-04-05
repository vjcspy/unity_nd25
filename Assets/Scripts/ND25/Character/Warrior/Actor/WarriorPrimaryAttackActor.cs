using Cysharp.Threading.Tasks;
using ND25.Core.ReactiveMachine;
using R3;
using System;
using UnityEngine;
namespace ND25.Character.Warrior.Actor
{
    public class WarriorPrimaryAttackActor : WarriorActorBase
    {

        public WarriorPrimaryAttackActor(WarriorMonoBehavior warriorMonoBehavior) : base(warriorMonoBehavior)
        {
            HandleContextChange();
        }

        void HandleContextChange()
        {
            warriorMonoBehavior.machine.ContextChangeHandler(context => context
                .Select(warriorContext =>
                {
                    warriorMonoBehavior.warriorAnimator.UpdateParam(WarriorAnimator.Param.primaryCombo, warriorContext.primaryCombo);

                    return Unit.Default;
                }));
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler PrimaryAttackTransition()
        {
            return upstream => upstream
                .OfAction(WarriorAction.PrimaryAttackTransition)
                .Select(
                    _ =>
                    {
                        if (!Input.GetKeyDown(KeyCode.J)) return ReactiveMachineCoreAction.Empty;

                        var machine = warriorMonoBehavior.machine;
                        machine.SetContext(
                            context =>
                            {
                                if (context.lastPrimaryAttackTime > Time.time - 0.2f && context.primaryCombo < 3)
                                {
                                    context.primaryCombo += 1;
                                }
                                else
                                {
                                    context.primaryCombo = 1;
                                }

                                return context;
                            }
                        );
                        machine.DispatchEvent(WarriorEvent.primaryAttack);

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }
    }
}
