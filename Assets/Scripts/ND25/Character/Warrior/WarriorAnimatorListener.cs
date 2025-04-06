using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
namespace ND25.Character.Warrior
{
    public class WarriorAnimatorListener : MonoBehaviour
    {
        WarriorReactiveMachine warriorReactiveMachine;
        void Awake()
        {
            warriorReactiveMachine = GetComponentInParent<WarriorReactiveMachine>();
        }
        public void OnFinishPrimaryAttack(int primaryCombo)
        {
            warriorReactiveMachine.machine.SetContext(context =>
            {
                context.lastPrimaryAttackTime = Time.time;

                return context;
            });
            warriorReactiveMachine.machine.DispatchEvent(WarriorEvent.idle);
        }
    }
}
