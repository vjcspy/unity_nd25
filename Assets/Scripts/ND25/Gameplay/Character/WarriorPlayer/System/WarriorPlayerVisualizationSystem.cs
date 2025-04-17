using ND25.Gameplay.Character.WarriorPlayer.Component;
using Unity.Entities;
using UnityEngine;
namespace ND25.Gameplay.Character.WarriorPlayer.System
{
    [UpdateInGroup(groupType: typeof(SimulationSystemGroup))]
    public partial struct WarriorPlayerVisualizationSystem : ISystem
    {
        private EntityQuery _entityQuery;

        public void OnCreate(ref SystemState state)
        {
            _entityQuery = SystemAPI.QueryBuilder()
                .WithAll<WarriorPlayerVisualizationRefData>()
                .WithNone<WarriorPlayerAnimationRefData>()
                .Build();

            state.RequireForUpdate(query: _entityQuery);
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = CreateECB(state: ref state);
            Entity playerEntity = _entityQuery.GetSingletonEntity();
            WarriorPlayerVisualizationRefData playerVisualizationRef = _entityQuery.GetSingleton<WarriorPlayerVisualizationRefData>();
            GameObject playerVisualizationObject = Object.Instantiate(original: playerVisualizationRef.gameObject);
            ecb.AddComponent(e: playerEntity, component: new WarriorPlayerAnimationRefData
            {
                animator = playerVisualizationObject.GetComponent<Animator>()
            });
        }

        private EntityCommandBuffer CreateECB(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            return ecbSingleton.CreateCommandBuffer(world: state.WorldUnmanaged);
        }
    }
}
