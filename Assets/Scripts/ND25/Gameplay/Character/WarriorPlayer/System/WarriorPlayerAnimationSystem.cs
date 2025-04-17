using ND25.Gameplay.Character.Common.Component;
using ND25.Gameplay.Character.Common.System;
using ND25.Gameplay.Character.WarriorPlayer.Component;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
namespace ND25.Gameplay.Character.WarriorPlayer.System
{
    [BurstCompile]
    [UpdateInGroup(groupType: typeof(SimulationSystemGroup))]
    [UpdateAfter(systemType: typeof(MoveSystem))]
    public partial struct WarriorPlayerAnimationSystem : ISystem
    {
        private EntityQuery _entityQuery;

        public void OnCreate(ref SystemState state)
        {
            _entityQuery = SystemAPI.QueryBuilder()
                .WithAll<LocalTransform>()
                .WithAll<MoveData>()
                .WithAll<WarriorPlayerAnimationData>()
                .Build();

            state.RequireForUpdate(query: _entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }
    }
}
