using System.Collections.Generic;
namespace ND25.Core.ReactiveMachine
{
    public class ReactiveMachineState
    {
        readonly ReactiveMachineStateConfig config;
        readonly ReactiveMachine machine;

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
            foreach (ReactiveMachineAction action in config.exit)
            {
                machine.DispatchAction(action);
            }
        }

        public void DispatchEvent(string eventName)
        {
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
