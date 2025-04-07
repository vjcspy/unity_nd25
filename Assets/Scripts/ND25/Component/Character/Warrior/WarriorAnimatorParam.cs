using ND25.Core.Utils;
using UnityEngine;
namespace ND25.Component.Character.Warrior
{
    public enum WarriorAnimatorParamName
    {
        idle,
        run,
        air,
        yVelocity,
        primaryCombo,
        primaryAttack,
    }

    public class WarriorAnimatorParam : AnimatorParamMap<WarriorAnimatorParamName>
    {
        public WarriorAnimatorParam(Animator animator) : base(animator)
        {
        }
    }
}
