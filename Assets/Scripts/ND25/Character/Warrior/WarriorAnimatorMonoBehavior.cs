using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
namespace ND25.Character.Warrior
{
    public class WarriorAnimatorMonoBehavior : MonoBehaviour
    {
        WarriorMonoBehavior warriorMonoBehavior;
        void Awake()
        {
            warriorMonoBehavior = GetComponentInParent<WarriorMonoBehavior>();
        }
        public void OnFinishPrimaryAttack(int primaryCombo)
        {
            warriorMonoBehavior.machine.SetContext(context =>
            {
                context.lastPrimaryAttackTime = Time.time;

                return context;
            });
            warriorMonoBehavior.machine.DispatchEvent(WarriorEvent.idle);
        }
    }
}
