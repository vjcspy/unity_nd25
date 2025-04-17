using ND25.Gameplay.Character.WarriorPlayer.Component;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
namespace ND25.Gameplay.Character.WarriorPlayer.System
{
    [UpdateInGroup(groupType: typeof(SimulationSystemGroup), OrderLast = true)]
    // [UpdateAfter(typeof(WarriorPlayerVisualizationSystem))]
    public partial struct WarriorPlayerAnimationSystem : ISystem
    {
        private EntityQuery _entityQuery;

        public void OnCreate(ref SystemState state)
        {
            _entityQuery = SystemAPI.QueryBuilder()
                .WithAll<LocalTransform>()
                .WithNone<WarriorPlayerMoveData>()
                .WithAll<WarriorPlayerAnimationRefData>()
                .Build();

            state.RequireForUpdate(query: _entityQuery);
        }

        public void OnUpdate(ref SystemState state)
        {
            LocalTransform localTransform = _entityQuery.GetSingleton<LocalTransform>();
            WarriorPlayerMoveData moveData = _entityQuery.GetSingleton<WarriorPlayerMoveData>();
            WarriorPlayerAnimationRefData animationRefData = _entityQuery.GetSingleton<WarriorPlayerAnimationRefData>();

            Transform transform = animationRefData.animator.transform;
            transform.position = localTransform.Position;
            transform.rotation = localTransform.Rotation;

            float scale = localTransform.Scale;
            transform.localScale = new Vector3(x: scale, y: scale, z: scale);
            Debug.Log(message: "here");
            animationRefData.animator.SetInteger(name: "state", value: 2);
        }
    }
}
