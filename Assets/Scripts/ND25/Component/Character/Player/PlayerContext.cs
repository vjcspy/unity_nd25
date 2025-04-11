using ND25.Util.Common.Enum;
namespace ND25.Component.Character.Player
{
    public class PlayerContext
    {
        public float lastJumpTime = 0f;
        public XDirection xInput = XDirection.None;
        public XDirection xVelocity = XDirection.None;
        public YDirection yVelocity = YDirection.None;
    }
}
