using System.Collections.Generic;
using UnityEngine;
namespace ND25.Core.ReactiveMachine
{
    public class ReactiveMachineState<T>
    {
        readonly ReactiveMachineStateConfig config;
        readonly ReactiveMachine<T> machine;

        public ReactiveMachineState(string stateName, ReactiveMachine<T> machine, ReactiveMachineStateConfig config)
        {
            name = stateName;
            this.machine = machine;
            this.config = config ?? new ReactiveMachineStateConfig();

            // Defensive init nếu không có entry/exit/invoke nào.
            this.config.entry ??= new List<ReactiveMachineAction>();
            this.config.exit ??= new List<ReactiveMachineAction>();
            this.config.invoke ??= new List<ReactiveMachineAction>();
            this.config.on ??= new Dictionary<string, List<StateTransition>>();
        }

        public string name
        {
            get;
        }


        public void Entry()
        {
            // #if UNITY_EDITOR
            // Debug.Log($"[ReactiveMachineState] Entry state: {name}");
            // #endif

            foreach (ReactiveMachineAction action in config.entry)
            {
                machine.DispatchAction(action);
            }
        }


        public void Invoke()
        {
            foreach (ReactiveMachineAction action in config.invoke)
            {
                machine.DispatchAction(action);
            }
        }

        public void Exit()
        {
            // #if UNITY_EDITOR
            // Debug.Log($"[ReactiveMachineState] Exit state: {name}");
            // #endif

            foreach (ReactiveMachineAction action in config.exit)
            {
                machine.DispatchAction(action);
            }
        }

        public void DispatchEvent(string eventName)
        {
            // #if UNITY_EDITOR
            // Debug.Log($"[ReactiveMachineState] Dispatch event: {eventName}");
            // #endif

            if (!config.on.TryGetValue(eventName, out var transitions) || transitions == null || transitions.Count == 0)
            {
                return;
            }

            // Hiện tại chỉ lấy transition đầu tiên nếu có nhiều lựa chọn.
            StateTransition transition = transitions[0];

            machine.TransitionTo(transition.target, transition.actions);
        }
    }
}
