using Unity.Entities;
using Unity.Mathematics;
namespace ND25.Gameplay.Character.WarriorPlayer.Component
{
    public struct WarriorPlayerAnimationData : IComponentData
    {
        public float3 position;
        public quaternion rotation;
        public float scale;
        public int animationState;
    }
}
