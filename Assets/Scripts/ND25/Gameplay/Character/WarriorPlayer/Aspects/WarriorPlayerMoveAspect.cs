using ND25.Gameplay.Character.WarriorPlayer.Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
namespace ND25.Gameplay.Character.WarriorPlayer.Aspects
{
    public readonly partial struct WarriorPlayerMoveAspect : IAspect
    {
        public readonly Entity entity;

        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<MoveComponentData> _movementData;

        public void Move(float2 direction, float deltaTime)
        {
            float3 move = new float3(x: direction.x, y: 0f, z: direction.y) * _movementData.ValueRO.moveSpeed * deltaTime;
            _transform.ValueRW.Position += move;
        }
    }
}
