using UnityEngine;
namespace ND25.Core.ReactiveMachine
{
    public abstract class ReactiveMachineWrapper<T> : MonoBehaviour
    {
        public ReactiveMachine<T> machine;

        protected virtual void Awake()
        {
            machine = new ReactiveMachine<T>(GetInitContext(), GetJsonFileName());
            machine.Awake();
        }

        void Start()
        {
            machine.Start();
            machine.RegisterActionHandler(GetActionHandlers());
            RegisterCustomerHandler();
        }

        void Update()
        {
            machine.Update();
        }

        void OnDestroy()
        {
            machine.OnDestroy();
        }

        protected virtual void RegisterCustomerHandler()
        {
            // Register custom handlers here if needed
        }

        protected abstract string GetJsonFileName();

        protected abstract object[] GetActionHandlers();
        protected abstract T GetInitContext();
    }
}
