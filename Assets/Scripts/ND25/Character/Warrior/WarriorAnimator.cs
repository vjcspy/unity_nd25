﻿using ND25.Core.Utils;
using UnityEngine;
namespace ND25.Character.Warrior
{
    public class WarriorAnimator : AnimatorParamMap<WarriorAnimator.Param>
    {
        public enum Param
        {
            idle,
            run,
            air,
            yVelocity
        }
        public WarriorAnimator(Animator animator) : base(animator)
        {
        }
    }
}
