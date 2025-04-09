using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace ND25.Core.XMachine
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class XMachineEffectAttribute : Attribute
    {
    }
    public delegate Observable<XMachineAction> XMachineActionHandler(Observable<XMachineAction> upstream);

    public class XMachineAction
    {
        public static readonly XMachineAction Empty = new XMachineAction("Empty");
        public XMachineAction(string type, Dictionary<string, object> payload = null)
        {
            this.type = type;
            this.payload = payload;
        }

        public XMachineAction(Enum type, Dictionary<string, object> payload = null)
        {
            this.type = type.ToString();
            this.payload = payload;
        }

        public Dictionary<string, object> payload { get; }

        public string type { get; }

        public static XMachineAction Create(Enum type, Dictionary<string, object> payload = null)
        {
            return new XMachineAction(type, payload);
        }
    }

    public class XMachineActionFactory<PayloadType>
    {
        public readonly string type;

        public XMachineActionFactory(string type)
        {
            this.type = type;
        }

        public XMachineAction Create(PayloadType payload)
        {
            var payloadDict = new Dictionary<string, object>
            {
                {
                    "data", payload
                }
            };

            return new XMachineAction(type, payloadDict);
        }
    }


    public abstract class XMachineState<T>
    {
        protected XMachineState(Enum id, XMachine<T> machine)
        {
            this.machine = machine;
            this.id = id;
        }

        XMachine<T> machine
        {
            get;
        }

        public Enum id
        {
            get;
        }

        public abstract HashSet<int> allowedEvents { get; }


        protected void InvokeAction(XMachineAction action)
        {
            machine.InvokeAction(action);
        }

        #region Logic

        // <summary>
        // Instead of wrapping logic trong các method xử lý thì sẽ để các inheritance tự handle
        // Kiểu gì sau này cũng sẽ cần custom nên làm càng simple càng dễ mở rộng
        // </summary>
        public abstract void Entry();

        public abstract void Update();

        public abstract void Exit();

        #endregion

    }

    public class XMachine<ContextType>
    {
        readonly Subject<XMachineAction> actionSubject = new Subject<XMachineAction>();
        readonly ReactiveProperty<ContextType> context;
        readonly Observable<XMachineAction> sharedActionStream;
        DisposableBag disposable;

        Dictionary<Enum, XMachineState<ContextType>> states;
        public XMachine(ContextType initialContext)
        {
            context = new ReactiveProperty<ContextType>(initialContext);
            sharedActionStream = actionSubject.Share();
        }
        ReactiveProperty<Enum> currentStateId { get; } = new ReactiveProperty<Enum>();

        public ContextType GetContext()
        {
            return context.Value;
        }

        public void InvokeAction(XMachineAction action)
        {
            actionSubject.OnNext(action);
        }

        public bool IsEventAllowed(int eventName)
        {
            return GetCurrentState().allowedEvents.Contains(eventName);
        }

        public Enum GetCurrentStateId()
        {
            return currentStateId.Value;
        }
        public XMachineState<ContextType> GetState(Enum id)
        {
            return states.GetValueOrDefault(id);
        }

        public XMachineState<ContextType> GetCurrentState()
        {
            return states[GetCurrentStateId()];
        }

        public XMachine<ContextType> Enable(Enum initialStateId = null)
        {
            initialStateId ??= states.First().Key;

            currentStateId.Value = initialStateId;
            GetCurrentState().Entry();

            return this;
        }

        public void Disable()
        {
            disposable.Dispose();
        }

        public XMachine<ContextType> RegisterStates(XMachineState<ContextType>[] machineStates)
        {
            if (machineStates == null || machineStates.Length == 0)
            {
                throw new ArgumentException("States cannot be null or empty");
            }

            states = machineStates.ToDictionary(state =>
            {
                return state.id;
            });

            return this;
        }

        public void Transition(Enum toStateId)
        {
            GetCurrentState().Exit();
            currentStateId.Value = toStateId;
            GetCurrentState().Entry();
        }

        public void RegisterAction(XMachineActionHandler eventHandler)
        {
            // Truyền trực tiếp subject vào handler để nhận stream xử lý
            eventHandler(sharedActionStream)
                .Where(handledEvent => handledEvent != XMachineAction.Empty)
                .Subscribe(
                    InvokeAction,
                    HandleError
                )
                .AddTo(ref disposable);
        }
        void HandleError(Result error)
        {
            Debug.LogError($"[ReactiveMachine] Error in event stream: {error}");
        }

        void RegisterActionHandler(XMachineEffect<ContextType> eventEffectInstance)
        {
            var methods = eventEffectInstance
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute<XMachineEffectAttribute>() != null && m.ReturnType == typeof(XMachineActionHandler))
                .ToList();

            foreach (XMachineActionHandler eventHandler in methods.Select(
                method =>
                    (XMachineActionHandler)method.Invoke(eventEffectInstance, null)!
            ))
            {
                RegisterAction(eventHandler);
            }
        }

        public void RegisterActionHandler(XMachineEffect<ContextType>[] actionEffectInstances)
        {
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var eventEffectInstance in actionEffectInstances)
            {
                RegisterActionHandler(eventEffectInstance);
            }
            sw.Stop();
            Debug.Log($"Time to register actions: {sw.ElapsedMilliseconds} ms");
        }


        public void SetContext(Func<ContextType, ContextType> contextUpdater)
        {
            // context.OnNext(contextUpdater(context.Value));
            contextUpdater(context.Value);
        }
    }

    public abstract class XMachineEffect<ContextType>
    {
        protected readonly XMachine<ContextType> machine;

        protected XMachineEffect(XMachine<ContextType> machine)
        {
            this.machine = machine;
        }

        protected ContextType context
        {
            get
            {
                return machine.GetContext();
            }
        }
    }

    public abstract class XMachineActor<ContextType> : MonoBehaviour
    {
        public XMachine<ContextType> machine { private set; get; }

        protected virtual void Awake()
        {
            machine = new XMachine<ContextType>(ConfigureInitialContext())
                .RegisterStates(ConfigureMachineStates());

        }

        protected virtual void Start()
        {
            machine.Enable(ConfigureInitialStateId());
        }

        protected void Update()
        {
            machine.GetCurrentState().Update();
        }

        protected void OnDestroy()
        {
            machine.Disable();
        }

        protected abstract ContextType ConfigureInitialContext();
        protected abstract XMachineState<ContextType>[] ConfigureMachineStates();
        protected abstract XMachineEffect<ContextType>[] ConfigureMachineEffects();
        protected abstract Enum ConfigureInitialStateId();
    }
}
