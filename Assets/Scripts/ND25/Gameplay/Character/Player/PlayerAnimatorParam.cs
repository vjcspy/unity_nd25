using ND25.Core.Utils;
using UnityEngine;
namespace ND25.Gameplay.Character.Player
{
    internal enum PlayerAnimatorState
    {
        Idle = 0,
        Move = 1,
        Air = 2,
        PrimaryAttack = 3,
        AimSword = 4,
        // ThrowSword = 5,
        CatchSword = 6,
    }

    internal enum PlayerAnimatorParamType
    {
        state,
        yVelocity,
        primaryAttackCount
    }

    internal class PlayerAnimatorParam : AnimatorParamMap<PlayerAnimatorParamType>
    {
        public PlayerAnimatorParam(Animator animator) : base(animator: animator)
        {
        }
    }
}
