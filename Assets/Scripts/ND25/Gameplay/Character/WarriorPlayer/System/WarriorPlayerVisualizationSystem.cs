using ND25.Core.ECS.Animation;
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
                .WithNone<WarriorPlayerAnimationData>()
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
            AnimationProxy proxy = playerVisualizationObject.GetComponent<AnimationProxy>();
            if (proxy != null)
            {
                proxy.entity = playerEntity;
            }
            else
            {
                Debug.LogError(message: "AnimationProxy component not found on the player visualization object.");
            }

            ecb.AddComponent(e: playerEntity, component: new WarriorPlayerAnimationData());
            ecb.AddComponent(e: playerEntity, component: new AnimationSyncData());
        }

        private EntityCommandBuffer CreateECB(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            return ecbSingleton.CreateCommandBuffer(world: state.WorldUnmanaged);
        }
    }
}
