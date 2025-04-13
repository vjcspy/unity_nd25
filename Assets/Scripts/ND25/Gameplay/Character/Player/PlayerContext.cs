using ND25.Util.Common.Enum;
namespace ND25.Gameplay.Character.Player
{
    public class PlayerContext
    {
        public float lastPrimaryAttackTime = 0f;
        public int primaryAttackCount = 0;

        public XDirection xInputDirection = XDirection.None;
        // public XDirection xVelocity = XDirection.None;
        public YDirection yVelocityDirection = YDirection.None;
    }
}
