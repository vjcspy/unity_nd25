using ND25.Gameplay.Character.Common.Component;
using ND25.Gameplay.Character.Common.System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
namespace ND25.Core.ECS.Animation
{
    [UpdateInGroup(groupType: typeof(SimulationSystemGroup))]
    [UpdateAfter(systemType: typeof(MoveSystem))]
    public partial struct AnimationSyncDataSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (localTransform, moveData, animData) in
                SystemAPI.Query<RefRO<LocalTransform>, RefRO<MoveData>, RefRW<AnimationSyncData>>()
                    .WithChangeFilter<LocalTransform>()
                // .WithChangeFilter<MoveData>()
                )
            {
                animData.ValueRW.position = new float3(
                    x: localTransform.ValueRO.Position.x + 0.4f,
                    y: localTransform.ValueRO.Position.y,
                    z: localTransform.ValueRO.Position.z
                );

                animData.ValueRW.rotation = localTransform.ValueRO.Rotation;
                animData.ValueRW.scale = localTransform.ValueRO.Scale;
            }
        }
    }
}
