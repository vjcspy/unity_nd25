using UnityEngine;
namespace ND25.Core.ReactiveMachine
{
    public abstract class ReactiveMachineMono<T> : MonoBehaviour
    {
        protected ReactiveMachine<T> machine;

        protected virtual void Awake()
        {
            machine = new ReactiveMachine<T>(GetInitContext(), GetJsonFileName());
            machine.Awake();
            machine.RegisterActionHandler(GetActionHandlers());
        }

        void Start()
        {
            machine.Start();
        }

        void Update()
        {
            machine.Update();
        }

        void OnDestroy()
        {
            machine.OnDestroy();
        }

        protected abstract string GetJsonFileName();

        protected abstract object[] GetActionHandlers();
        protected abstract T GetInitContext();
    }
}
