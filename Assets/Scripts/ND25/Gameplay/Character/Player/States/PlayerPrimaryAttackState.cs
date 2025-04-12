using ND25.Core.XMachine;
using System;
using UnityEngine;
namespace ND25.Gameplay.Character.Player.States
{
    public class PlayerPrimaryAttackState : XMachineState<PlayerContext>
    {

        private int nextAttackCount;
        public PlayerPrimaryAttackState(Enum id, PlayerActor actor) : base(id: id, actor: actor)
        {
        }

        internal override void Entry()
        {
            if (actor.machine.GetContextValue().lastPrimaryAttackTime < Time.time - 0.2f || actor.machine.GetContextValue().primaryAttackCount > 2)
            {
                SetContext(contextUpdater: context =>
                {
                    context.primaryAttackCount = 0;
                });
            }

            if (actor.machine.GetContextValue().primaryAttackCount == 2)
            {
                InvokeAction(action: PlayerAction.ForceJump.SetFloat(key: PlayerAction.DataKey.JumpForce, value: 3));
            }
        }
        internal override void Exit()
        {
            SetContext(contextUpdater: context =>
            {
                context.lastPrimaryAttackTime = Time.time;
                context.primaryAttackCount += 1;
            });
        }

        internal override void OnAnimationFinish()
        {
            actor.machine.Transition(toStateId: PlayerState.Idle);
        }
    }
}
