namespace ND25.Character.Warrior
{
    internal enum WarriorAction
    {
        HandleXInput,
        UpdateAnimatorParams,
        ForceJump,

        WhenXInputChange, // When naming convention, use "When" prefix for dispatching events
        WhenFallGround,   // When naming convention, use "When" prefix for dispatching events
        WhenYInputChange  // When naming convention, use "When" prefix for dispatching events
    }
}
