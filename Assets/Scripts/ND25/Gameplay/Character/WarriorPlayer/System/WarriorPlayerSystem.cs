using Unity.Entities;
namespace ND25.Gameplay.Character.WarriorPlayer.System
{
    // [BurstCompile]                                 // Tạm gỡ dòng này nếu muốn log
    [UpdateInGroup(groupType: typeof(SimulationSystemGroup))] // Nhớ group này phải tồn tại
    public partial struct WarriorPlayerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
        }
    }
}
