using ND25.Core.Utils;
using System.Collections.Generic;
using UnityEngine;
namespace ND25.Component.Character.Skeleton
{
    public enum SkeletonAnimatorParamName
    {
        state
    }

    public class SkeletonAnimatorParam : AnimatorParamMap<SkeletonAnimatorParamName>
    {
        public Dictionary<string, int> paramValueMap = new Dictionary<string, int>
        {
            {
                "Idle", 0
            },
            {
                "Move", 1
            }
        };
        public SkeletonAnimatorParam(Animator animator) : base(animator)
        {
        }
    }
}
