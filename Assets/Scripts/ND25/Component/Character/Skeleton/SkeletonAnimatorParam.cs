using ND25.Core.Utils;
using UnityEngine;
namespace ND25.Component.Character.Skeleton
{
    public enum SkeletonAnimatorParamName
    {
        state,
    }

    public class SkeletonAnimatorParam: AnimatorParamMap<SkeletonAnimatorParamName>
    {

        public SkeletonAnimatorParam(Animator animator) : base(animator)
        {
        }
    }
}
