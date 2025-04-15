using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
namespace ND25.Core.EMachine
{
    [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl)]
    public delegate void EnterDelegate(ref SystemState state);

    [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl)]
    public delegate void ExitDelegate(ref SystemState state);
    [UnmanagedFunctionPointer(callingConvention: CallingConvention.Cdecl)]
    public delegate bool GuardDelegate(ref SystemState state);

    public enum EMachineTransitionState
    {
        GuardNextState,
        ExitPrevious,
        EnterNextState
    }
    public struct EMachineComponent : IComponentData
    {
        public int prevState;
        public int currentState;
        public int nextState;

        public int transitionState;

        public static EMachineComponent Default
        {
            get
            {
                return new EMachineComponent
                {
                    prevState = -1,
                    currentState = -1,
                    nextState = -1,
                    transitionState = (int)EMachineTransitionState.GuardNextState
                };
            }
        }
    }

    [BurstCompile]
    public class EMachine
    {

        [BurstCompile]
        public static void Transition(ref SystemState state,ref Entity entity, int nextState)
        {
            // Get the EMachineComponent from the entity
            EMachineComponent emachine = state.EntityManager.GetComponentData<EMachineComponent>(entity: entity);
            // Set the next state
            emachine.nextState = nextState;
            emachine.transitionState = (int)EMachineTransitionState.GuardNextState;
            // Update the EMachineComponent
            state.EntityManager.SetComponentData(entity: entity, componentData: emachine);
        }

        // eMachineState: register for this state
        // enter: the function to call when entering the new state (enable the component data for this state)
        // exit: the function to call when exiting the current state (disable the component data for this state)
        // guard: the function to call when checking if the state can be entered (check if the component data is valid)
        // This function will be called in each State System.
        [BurstCompile]
        public static void Update(
            ref SystemState state,
            ref Entity entity,
            int eMachineState,
            FunctionPointer<EnterDelegate> enter = default(FunctionPointer<EnterDelegate>),
            FunctionPointer<ExitDelegate> exit = default(FunctionPointer<ExitDelegate>),
            FunctionPointer<GuardDelegate> guard = default(FunctionPointer<GuardDelegate>)
        )
        {
            EMachineComponent eMachineComponent = state.EntityManager.GetComponentData<EMachineComponent>(entity: entity);

            switch (eMachineComponent.nextState)
            {
                case > 0 when eMachineComponent.transitionState == (int)EMachineTransitionState.GuardNextState:
                    {
                        if (eMachineComponent.nextState == eMachineState)
                        {
                            bool result = true;

                            // Nếu có guard function thì gọi
                            if (guard.IsCreated)
                                result = guard.Invoke(state: ref state);

                            if (result)
                            {
                                eMachineComponent.transitionState = (int)EMachineTransitionState.ExitPrevious;
                                state.EntityManager.SetComponentData(entity: entity, componentData: eMachineComponent);
                            }
                        }
                        break;
                    }

                case > 0 when eMachineComponent.transitionState == (int)EMachineTransitionState.ExitPrevious:
                    {
                        if (eMachineComponent.currentState == eMachineState)
                        {
                            // Nếu có exit function thì gọi
                            if (exit.IsCreated)
                                exit.Invoke(state: ref state);

                            eMachineComponent.transitionState = (int)EMachineTransitionState.EnterNextState;
                            state.EntityManager.SetComponentData(entity: entity, componentData: eMachineComponent);
                        }
                        break;
                    }

                case > 0 when eMachineComponent.transitionState == (int)EMachineTransitionState.EnterNextState:
                    {
                        if (eMachineComponent.nextState == eMachineState)
                        {
                            // Nếu có enter function thì gọi
                            if (enter.IsCreated)
                                enter.Invoke(state: ref state);

                            eMachineComponent.prevState = eMachineComponent.currentState;
                            eMachineComponent.currentState = eMachineComponent.nextState;
                            eMachineComponent.nextState = -1;
                            eMachineComponent.transitionState = (int)EMachineTransitionState.GuardNextState;

                            state.EntityManager.SetComponentData(entity: entity, componentData: eMachineComponent);
                        }
                        break;
                    }
            }
        }
    }
}
