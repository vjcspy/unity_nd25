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
    [AttributeUsage(validOn: AttributeTargets.Method, Inherited = false)]
    public class XMachineEffectAttribute : Attribute
    {
    }
    public delegate Observable<XMachineAction> XMachineActionHandler(Observable<XMachineAction> upstream);

    public class XMachineAction
    {
        public static readonly XMachineAction Empty = new XMachineAction(type: "Empty");
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
            return new XMachineAction(type: type, payload: payload);
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
                { "data", payload }
            };

            return new XMachineAction(type: type, payload: payloadDict);
        }
    }


    public abstract class XMachineState<ContextType>
    {
        protected XMachineState(Enum id, XMachineActor<ContextType> actor)
        {
            this.actor = actor;
            this.id = id;
        }

        private XMachineActor<ContextType> actor
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
            actor.machine.InvokeAction(action: action);
        }

        public void SetContext(Func<ContextType, ContextType> contextUpdater)
        {
            actor.machine.SetContext(contextUpdater: contextUpdater);
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
        private readonly Subject<XMachineAction> actionSubject = new Subject<XMachineAction>();
        private readonly ReactiveProperty<ContextType> context;
        private readonly Observable<XMachineAction> sharedActionStream;
        private DisposableBag disposable;

        private Dictionary<Enum, XMachineState<ContextType>> states;
        public XMachine(ContextType initialContext)
        {
            context = new ReactiveProperty<ContextType>(value: initialContext);
            sharedActionStream = actionSubject.Share();
        }
        private ReactiveProperty<Enum> currentStateId { get; } = new ReactiveProperty<Enum>();

        public ContextType GetContext()
        {
            return context.Value;
        }

        public void InvokeAction(XMachineAction action)
        {
            actionSubject.OnNext(value: action);
        }

        public bool IsEventAllowed(int eventName)
        {
            return GetCurrentState().allowedEvents.Contains(item: eventName);
        }

        public Enum GetCurrentStateId()
        {
            return currentStateId.Value;
        }
        public XMachineState<ContextType> GetState(Enum id)
        {
            return states.GetValueOrDefault(key: id);
        }

        public XMachineState<ContextType> GetCurrentState()
        {
            return states[key: GetCurrentStateId()];
        }

        public XMachine<ContextType> Start(Enum initialStateId = null)
        {
            initialStateId ??= states.First().Key;

            currentStateId.Value = initialStateId;
            Debug.Log("Entering initial state: " + initialStateId);
            GetCurrentState().Entry();

            return this;
        }

        public void Stop()
        {
            disposable.Dispose();
        }

        public XMachine<ContextType> RegisterStates(XMachineState<ContextType>[] machineStates)
        {
            if (machineStates == null || machineStates.Length == 0)
            {
                throw new ArgumentException(message: "States cannot be null or empty");
            }

            states = machineStates.ToDictionary(keySelector: state => state.id);

            return this;
        }

        public void Transition(Enum toStateId)
        {
            Debug.Log("Exiting state: " + GetCurrentStateId());
            GetCurrentState().Exit();
            currentStateId.Value = toStateId;
            Debug.Log("Entering state: " + toStateId);
            GetCurrentState().Entry();
        }

        public XMachine<ContextType> RegisterAction(XMachineActionHandler eventHandler)
        {
            Debug.Log("Registering action: " + eventHandler.GetMethodInfo().Name);
            // Truyền trực tiếp subject vào handler để nhận stream xử lý
            eventHandler(upstream: sharedActionStream)
                .Where(predicate: handledEvent => handledEvent != XMachineAction.Empty)
                .Subscribe(
                    onNext: InvokeAction,
                    onCompleted: HandleError
                )
                .AddTo(bag: ref disposable);

            return this;
        }
        private void HandleError(Result error)
        {
            Debug.LogError(message: $"[ReactiveMachine] Error in event stream: {error}");
        }

        private XMachine<ContextType> RegisterActionHandler(XMachineEffect<ContextType> eventEffectInstance)
        {
            var methods = eventEffectInstance
                .GetType()
                .GetMethods()
                .Where(predicate: m => m.GetCustomAttribute<XMachineEffectAttribute>() != null && m.ReturnType == typeof(XMachineActionHandler))
                .ToList();

            foreach (XMachineActionHandler eventHandler in methods.Select(
                selector: method =>
                    (XMachineActionHandler)method.Invoke(obj: eventEffectInstance, parameters: null)!
            ))
            {
                RegisterAction(eventHandler: eventHandler);
            }

            return this;
        }

        public XMachine<ContextType> RegisterActionHandler(XMachineEffect<ContextType>[] actionEffectInstances)
        {
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var eventEffectInstance in actionEffectInstances)
            {
                RegisterActionHandler(eventEffectInstance: eventEffectInstance);
            }
            sw.Stop();
            Debug.Log(message: $"Time to register actions: {sw.ElapsedMilliseconds} ms");

            return this;
        }


        public void SetContext(Func<ContextType, ContextType> contextUpdater)
        {
            // context.OnNext(contextUpdater(context.Value));
            contextUpdater(arg: context.Value);
        }
    }

    public abstract class XMachineEffect<ContextType>
    {
        protected readonly XMachineActor<ContextType> actor;

        protected XMachineEffect(XMachineActor<ContextType> actor)
        {
            this.actor = actor;
        }

        protected ContextType context
        {
            get
            {
                return actor.machine.GetContext();
            }
        }
    }

    public abstract class XMachineActor<ContextType> : MonoBehaviour
    {
        public XMachine<ContextType> machine { private set; get; }

        protected virtual void Awake()
        {
            machine = new XMachine<ContextType>(initialContext: ConfigureInitialContext())
                .RegisterStates(machineStates: ConfigureMachineStates())
                .RegisterActionHandler(actionEffectInstances: ConfigureMachineEffects());
        }

        protected virtual void Start()
        {
            machine.Start(initialStateId: ConfigureInitialStateId());
        }

        protected void Update()
        {
            machine.GetCurrentState().Update();
        }

        protected void OnDestroy()
        {
            machine.Stop();
        }

        protected abstract ContextType ConfigureInitialContext();
        protected abstract XMachineState<ContextType>[] ConfigureMachineStates();
        protected abstract XMachineEffect<ContextType>[] ConfigureMachineEffects();
        protected abstract Enum ConfigureInitialStateId();
    }
}
