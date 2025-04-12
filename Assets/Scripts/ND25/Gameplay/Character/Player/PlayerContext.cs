using ND25.Util.Common.Enum;
namespace ND25.Gameplay.Character.Player
{
    public class PlayerContext
    {
        public float lastPrimaryAttackTime = 0f;
        public int primaryAttackCount = 0;

        public XDirection xInput = XDirection.None;
        // public XDirection xVelocity = XDirection.None;
        public YDirection yVelocity = YDirection.None;
    }
}
