using Unity.Entities;
using Unity.Mathematics;
namespace ND25.Core.ECS.Animation
{
    public struct AnimationSyncData : IComponentData
    {
        public float3 position;
        public quaternion rotation;
        public float scale;
    }
}
