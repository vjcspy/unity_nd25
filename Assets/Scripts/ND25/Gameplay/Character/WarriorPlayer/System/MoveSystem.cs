using ND25.Gameplay.Character.WarriorPlayer.Aspects;
using ND25.Gameplay.Character.WarriorPlayer.Component;
using ND25.Input.InputECS;
using Unity.Burst;
using Unity.Entities;
namespace ND25.Gameplay.Character.WarriorPlayer.System
{
    [BurstCompile]
    public partial struct MoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // Đăng ký các thành phần cần thiết
            state.RequireForUpdate<PlayerInputData>();
            state.RequireForUpdate<WarriorPlayerMoveData>();
            state.RequireForUpdate<WarriorPlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            // Job để xử lý di chuyển
            new PlayerMovementJob
            {
                DeltaTime = deltaTime
            }.ScheduleParallel();
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
