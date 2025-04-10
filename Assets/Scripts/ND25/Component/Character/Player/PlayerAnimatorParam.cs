using ND25.Core.Utils;
using UnityEngine;
namespace ND25.Component.Character.Player
{
    internal enum PlayerAnimatorState
    {
        Idle = 0,
        Move = 1,
        Air = 2,
        PrimaryAttack = 3
    }

    internal enum PlayerAnimatorParamType
    {
        state,
        yVelocity,
        primaryCount
    }

    internal class PlayerAnimatorParam : AnimatorParamMap<PlayerAnimatorParamType>
    {
        public PlayerAnimatorParam(Animator animator) : base(animator: animator)
        {
        }
    }
}
