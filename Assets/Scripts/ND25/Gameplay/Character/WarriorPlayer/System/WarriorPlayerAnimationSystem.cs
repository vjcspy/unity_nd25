using ND25.Gameplay.Character.WarriorPlayer.Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
namespace ND25.Gameplay.Character.WarriorPlayer.System
{
    [UpdateInGroup(groupType: typeof(SimulationSystemGroup))]
    [UpdateAfter(systemType: typeof(MoveSystem))]
    public partial struct WarriorPlayerAnimationSystem : ISystem
    {
        private EntityQuery _entityQuery;

        public void OnCreate(ref SystemState state)
        {
            _entityQuery = SystemAPI.QueryBuilder()
                .WithAll<LocalTransform>()
                .WithAll<WarriorPlayerMoveData>()
                .WithAll<WarriorPlayerAnimationRefData>()
                .Build();

            state.RequireForUpdate(query: _entityQuery);
        }

        public void OnUpdate(ref SystemState state)
        {
            state.CompleteDependency();
            LocalTransform localTransform = _entityQuery.GetSingleton<LocalTransform>();
            WarriorPlayerMoveData moveData = _entityQuery.GetSingleton<WarriorPlayerMoveData>();
            WarriorPlayerAnimationRefData animationRefData = _entityQuery.GetSingleton<WarriorPlayerAnimationRefData>();

            Transform transform = animationRefData.animator.transform;

            // Thêm offset 0.4f vào trục x để đúng pivot
            float3 correctedPosition = localTransform.Position;
            correctedPosition.x += 0.4f;

            transform.position = new Vector3(x: correctedPosition.x, y: correctedPosition.y, z: correctedPosition.z);
            transform.rotation = localTransform.Rotation;

            float scale = localTransform.Scale;
            transform.localScale = new Vector3(x: scale, y: scale, z: scale);

            animationRefData.animator.SetInteger(name: "state", value: 1);
        }
    }
}
