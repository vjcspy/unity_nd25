using ND25.Util.Common.Enum;
using Unity.Entities;
namespace ND25.Gameplay.Character.Entity
{
    public interface EntityXDirection
    {
        public XDirection GetCurrentFacingDirection();
    }

    public class EnemyInfo
    {
        public int health;
    }

    public struct EnemyComponent : IComponentData
    {
    }
}
