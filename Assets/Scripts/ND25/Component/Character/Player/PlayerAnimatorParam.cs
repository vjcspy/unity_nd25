using ND25.Core.Utils;
using UnityEngine;
namespace ND25.Component.Character.Player
{
    internal enum PlayerAnimatorState
    {
        Idle = 0,
        Move = 1
    }

    internal enum PlayerAnimatorParamType
    {
        state = 0,
        yVelocity = 1,
        primaryCount = 2
    }

    internal class PlayerAnimatorParam : AnimatorParamMap<PlayerAnimatorParamType>
    {
        public PlayerAnimatorParam(Animator animator) : base(animator: animator)
        {
        }
    }
}
