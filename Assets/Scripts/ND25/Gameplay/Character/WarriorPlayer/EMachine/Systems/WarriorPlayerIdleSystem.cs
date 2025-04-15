using ND25.Core.EMachine;
using Unity.Burst;
using Unity.Entities;
namespace ND25.Gameplay.Character.WarriorPlayer.EMachine.Systems
{
    [BurstCompile]
    public partial struct WarriorPlayerIdleSystem : ISystem
    {
        private FunctionPointer<GuardDelegate> _guardFn;
        private FunctionPointer<EnterDelegate> _enterFn;
        private FunctionPointer<ExitDelegate> _exitFn;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _guardFn = BurstCompiler.CompileFunctionPointer<GuardDelegate>(delegateMethod: Guard);
            _enterFn = BurstCompiler.CompileFunctionPointer<EnterDelegate>(delegateMethod: Enter);
            _exitFn = BurstCompiler.CompileFunctionPointer<ExitDelegate>(delegateMethod: Exit);
        }

        [BurstCompile]
        private static bool Guard(ref SystemState state)
        {
            return true;
        }

        [BurstCompile]
        private static void Enter(ref SystemState state)
        {
        }

        [BurstCompile]
        private static void Exit(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((_, Entity readonlyEntity) in SystemAPI.Query<RefRO<EMachineComponent>>().WithEntityAccess())
            {
                Entity entity = readonlyEntity;

                Core.EMachine.EMachine.Update(
                    state: ref state,
                    entity: ref entity,
                    eMachineState: (int)WarriorPlayerState.Idle,
                    guard: _guardFn,
                    enter: _enterFn,
                    exit: _exitFn
                );
            }
        }
    }
}
