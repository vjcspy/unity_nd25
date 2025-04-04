using UnityEngine;
namespace ND25.Core.ReactiveMachine
{
    public abstract class ReactiveMachineMono : MonoBehaviour
    {
        ReactiveMachine machine;

        protected virtual void Awake()
        {
            machine = new ReactiveMachine(GetJsonFileName());
            machine.Awake();
            machine.RegisterActionHandler(GetActionHandlers());
        }

        // void Start()
        // {
        //     machine.Start();
        // }
        //
        // void Update()
        // {
        //     machine.Update();
        // }

        void OnDestroy()
        {
            machine.OnDestroy();
        }

        protected abstract string GetJsonFileName();

        protected abstract object[] GetActionHandlers();
    }
}
