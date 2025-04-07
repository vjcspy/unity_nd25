﻿using ND25.Core.ReactiveMachine;
using R3;
using UnityEngine;
namespace ND25.Component.Character.Warrior.Actor
{
    public class WarriorPrimaryAttackActor : WarriorActorBase
    {

        public WarriorPrimaryAttackActor(WarriorReactiveMachine warriorReactiveMachine) : base(warriorReactiveMachine)
        {
            HandleContextChange();
        }

        void HandleContextChange()
        {
            warriorReactiveMachine.machine.ContextChangeHandler(context => context
                .Select(warriorContext =>
                {
                    warriorReactiveMachine.animatorParam.UpdateParam(WarriorAnimatorParamName.primaryCombo, warriorContext.primaryCombo);

                    return Unit.Default;
                }));
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler PrimaryAttackTransition()
        {
            return upstream => upstream
                .OfAction(WarriorActionType.PrimaryAttackTransition)
                .Select(
                    _ =>
                    {
                        if (!Input.GetKeyDown(KeyCode.J)) return ReactiveMachineCoreAction.Empty;

                        var machine = warriorReactiveMachine.machine;
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

                        return ReactiveMachineCoreAction.TransitionActionFactory.Create(WarriorEvent.primaryAttack);
                    }
                );
        }
    }
}
