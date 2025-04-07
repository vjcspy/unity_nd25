namespace ND25.Component.Character.Skeleton.Actor
{
    public abstract class SkeletonActorBase
    {
        protected readonly SkeletonReactiveMachine reactiveMachineComponent;
        protected SkeletonActorBase(SkeletonReactiveMachine reactiveMachineComponent)
        {
            this.reactiveMachineComponent = reactiveMachineComponent;
        }
    }
}
