using System.Collections.Generic;
using UnityEngine;

namespace ND25.Core.ReactiveMachine
{
    public class ReactiveMachineState
    {
        private readonly ReactiveMachineStateConfig config;
        private readonly ReactiveMachine machine;

        public ReactiveMachineState(ReactiveMachine machine, ReactiveMachineStateConfig config)
        {
            this.machine = machine;
            this.config = config ?? new ReactiveMachineStateConfig();

            // Defensive init nếu không có entry/exit/invoke nào.
            this.config.entry ??= new List<ReactiveMachineAction>();
            this.config.exit ??= new List<ReactiveMachineAction>();
            this.config.invoke ??= new List<ReactiveMachineAction>();
            this.config.on ??= new Dictionary<string, List<StateTransition>>();
        }


        public void Entry()
        {
#if UNITY_EDITOR
            Debug.Log("[FSM State] Entry");
#endif

            foreach (var action in config.entry) machine.DispatchAction(action);

        }


        public void Invoke()
        {
            foreach (ReactiveMachineAction action in config.invoke) machine.DispatchAction(action);
        }

        public void Exit()
        {
#if UNITY_EDITOR
            Debug.Log("[FSM State] Exit");
#endif

            foreach (var action in config.exit) machine.DispatchAction(action);
        }

        /// <summary>
        ///     Khi nhận event từ bên ngoài dispatch vào FSM.
        /// </summary>
        /// <param name="eventName">Tên event như "jump", "move"...</param>
        public void DispatchEvent(string eventName)
        {
#if UNITY_EDITOR
            Debug.Log($"[FSM State] Handling Event: {eventName}");
#endif

            if (!config.on.TryGetValue(eventName, out var transitions) || transitions == null || transitions.Count == 0)
                return;

            // Hiện tại chỉ lấy transition đầu tiên nếu có nhiều lựa chọn.
            var transition = transitions[0];

#if UNITY_EDITOR
            Debug.Log($"[FSM State] Event '{eventName}' matched. Transition to '{transition.target}'");
#endif

            machine.TransitionTo(transition.target, transition.actions);
        }
    }
}