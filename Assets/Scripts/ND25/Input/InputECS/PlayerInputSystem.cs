using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace ND25.Input.InputECS
{
    [UpdateInGroup(groupType: typeof(InitializationSystemGroup))]
    public partial class PlayerInputSystem : SystemBase
    {
        private PCControls _pcControls;

        protected override void OnCreate()
        {
            RequireForUpdate<PlayerInputData>();

            _pcControls = new PCControls();
            _pcControls.GamePlay.Enable();
        }

        protected override void OnDestroy()
        {
            _pcControls.GamePlay.Disable();
        }

        protected override void OnUpdate()
        {
            // Đọc input từ Input System
            Vector2 moveVec = _pcControls.GamePlay.Move.ReadValue<Vector2>();
            float2 move = new float2(x: moveVec.x, y: moveVec.y);

            // Tạo job và schedule mà không cần EntityQuery nhờ có [WithAll]
            UpdatePlayerInputJob job = new UpdatePlayerInputJob
            {
                moveInput = move
            };

            Dependency = job.ScheduleParallel(dependsOn: Dependency);
        }
    }

    [BurstCompile]
    [WithAll(typeof(PlayerInputData))]
    public partial struct UpdatePlayerInputJob : IJobEntity
    {
        public float2 moveInput;

        private void Execute(ref PlayerInputData input)
        {
            input.moveInput = moveInput;
        }
    }
}
