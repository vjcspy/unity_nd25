namespace ND25.Character.Warrior.Actor
{
    public abstract class WarriorActorBase
    {
        protected readonly WarriorMonoBehavior warriorMonoBehavior;
        protected WarriorActorBase(WarriorMonoBehavior warriorMonoBehavior)
        {
            this.warriorMonoBehavior = warriorMonoBehavior;
        }
    }
}
