using ND25.Util.Common.Enum;
namespace ND25.Gameplay.Character.Common.MethodInterface
{
    public interface IFacingDirection
    {
        public XDirection GetCurrentFacingDirection();
        public void SetCurrentFacingDirection(XDirection direction);
    }
}
