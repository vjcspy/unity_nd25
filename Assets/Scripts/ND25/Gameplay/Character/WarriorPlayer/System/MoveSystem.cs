using ND25.Gameplay.Character.WarriorPlayer.Aspects;
using ND25.Gameplay.Character.WarriorPlayer.Component;
using ND25.Input.InputECS;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
namespace ND25.Gameplay.Character.WarriorPlayer.System
{
    [BurstCompile]
    [UpdateInGroup(groupType: typeof(SimulationSystemGroup))]
    public partial struct MoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInputData>();
            state.RequireForUpdate<WarriorPlayerMoveData>();
            state.RequireForUpdate<WarriorPlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            PlayerMovementJob job = new PlayerMovementJob
            {
                DeltaTime = deltaTime
            };

            // Lên lịch job và gán phụ thuộc
            JobHandle jobHandle = job.ScheduleParallel(dependsOn: state.Dependency);
            state.Dependency = jobHandle;
        }
    }

    [BurstCompile]
    [WithAll(typeof(WarriorPlayerTag))]
    public partial struct PlayerMovementJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(WarriorPlayerMoveAspect aspect)
        {
            aspect.Move(deltaTime: DeltaTime);
        }
    }
}
