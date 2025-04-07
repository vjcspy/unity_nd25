using ND25.Component.Character.Common;
using ND25.Core.ReactiveMachine;
using UnityEngine;
namespace ND25.Component.Character.Skeleton
{
    public class SkeletonReactiveMachine : ReactiveMachineComponent<SkeletonContext>
    {
        Animator animator;
        SkeletonAnimatorParam animatorParam;
        Rigidbody2D rb;

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            animatorParam = new SkeletonAnimatorParam(animator);
        }

        protected override string GetJsonFileName()
        {
            return "skeleton";
        }
        protected override object[] GetActionHandlers()
        {
            return new object[]
            {
            };
        }
        protected override SkeletonContext GetInitContext()
        {
            return new SkeletonContext();
        }

        protected override void RegisterCustomerHandler()
        {
            machine.RegisterAction(CommonActor.UpdateAnimatorParams(animatorParam));
        }
    }
}
