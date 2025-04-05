namespace ND25.Character.Warrior
{
    public class WarriorContext
    {

        public float lastJumpTime;
        public float yVelocity;


        public WarriorContext(float lastJumpTime = 0f, float yVelocity = 0f)
        {
            this.yVelocity = yVelocity;
            this.lastJumpTime = lastJumpTime;
        }
    }
}
