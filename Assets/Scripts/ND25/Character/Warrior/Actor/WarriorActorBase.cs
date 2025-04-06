namespace ND25.Character.Warrior.Actor
{
    public abstract class WarriorActorBase
    {
        protected readonly WarriorReactiveMachine warriorReactiveMachine;
        protected WarriorActorBase(WarriorReactiveMachine warriorReactiveMachine)
        {
            this.warriorReactiveMachine = warriorReactiveMachine;
        }
    }
}
