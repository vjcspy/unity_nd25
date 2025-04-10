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

    [AttributeUsage(validOn: AttributeTargets.Method, Inherited = false)]
    public class XMachineSubscribeAttribute : Attribute
    {

        public XMachineSubscribeAttribute(params object[] subscribedTypes)
        {
            SubscribedTypes = new HashSet<string>(collection: subscribedTypes.Select(selector: type =>
            {
                switch (type)
                {
                    case Enum enumValue:
                        return enumValue.ToString();

                    case string str:
                        return str;

                    case Type t when typeof(XMachineAction).IsAssignableFrom(c: t):
                        {
                            // Tạo một instance giả định để lấy giá trị "type"
                            try
                            {
                                XMachineAction instance = Activator.CreateInstance(type: t) as XMachineAction;
                                if (instance == null || string.IsNullOrEmpty(value: instance.type))
                                    throw new ArgumentException(message: $"Class '{t.Name}' must have non-null type property.");
                                return instance.type;
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentException(message: $"Failed to instantiate or extract 'type' from {t.Name}: {ex.Message}");
                            }
                        }

                    default:
                        throw new ArgumentException(message: "Subscribed types must be Enum, string, or subclass of XMachineAction.");
                }
            }));
        }
        public HashSet<string> SubscribedTypes { get; }
    }

    public delegate Observable<XMachineAction> XMachineActionHandler(Observable<XMachineAction> upstream);
    public delegate void XMachineActionSubscriber(XMachineAction action);

    public class XMachineAction
    {
        public static readonly XMachineAction Empty = new XMachineAction(type: "Empty");
        public static readonly XMachineAction Transition = new XMachineAction(type: "Transition");
        public XMachineAction(string type, object payload = null)
        {
            this.type = type;
            this.payload = payload;
        }

        public XMachineAction(Enum type, object payload = null)
        {
            this.type = type.ToString();
            this.payload = payload;
        }

        public object payload { get; }

        public string type { get; }

        public XMachineAction Factory(object data)
        {
            return new XMachineAction(type: type, payload: data);
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

        public abstract void FixedUpdate();
        public abstract void Update();

        public abstract void Exit();

        public virtual bool SelfTransition()
        {
            return false;
        }

        #endregion

    }

    public class XMachine<ContextType>
    {
        private readonly Subject<XMachineAction> actionSubject = new Subject<XMachineAction>();
        public readonly ReactiveProperty<ContextType> reactiveContext;
        private readonly Observable<XMachineAction> sharedActionStream;
        private DisposableBag disposable;

        private Dictionary<Enum, XMachineState<ContextType>> states;
        public XMachine(ContextType initialContext)
        {
            reactiveContext = new ReactiveProperty<ContextType>(value: initialContext);
            sharedActionStream = actionSubject.Share();
        }
        public ReactiveProperty<Enum> reactiveCurrentStateId { get; } = new ReactiveProperty<Enum>();

        public ContextType GetContextValue()
        {
            return reactiveContext.Value;
        }

        public void InvokeAction(XMachineAction action)
        {
            if (action.type == XMachineAction.Transition.type)
            {
                Transition(toStateId: (Enum)action.payload);
                return;
            }
            actionSubject.OnNext(value: action);
        }

        public bool IsEventAllowed(int eventName)
        {
            return GetCurrentState().allowedEvents.Contains(item: eventName);
        }

        public Enum GetCurrentStateId()
        {
            return reactiveCurrentStateId.Value;
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

            reactiveCurrentStateId.Value = initialStateId;
            Debug.Log(message: "Entering initial state: " + initialStateId);
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
            if (Equals(objA: toStateId, objB: GetCurrentStateId()) && !GetCurrentState().SelfTransition())
            {
                return;
            }

            GetCurrentState().Exit();
            reactiveCurrentStateId.Value = toStateId;
            GetCurrentState().Entry();
        }

        public XMachine<ContextType> RegisterAction(XMachineActionHandler eventHandler)
        {
            // Debug.Log("Registering action: " + eventHandler.GetMethodInfo().Name);
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

        public XMachine<ContextType> RegisterEffects(XMachineEffect<ContextType>[] actionEffectInstances)
        {
            Stopwatch sw = Stopwatch.StartNew();
            foreach (var eventEffectInstance in actionEffectInstances)
            {
                RegisterActionHandler(eventEffectInstance: eventEffectInstance);
                RegisterActionSubscriber(eventEffectInstance: eventEffectInstance);
            }
            sw.Stop();
            Debug.Log(message: $"Time to register actions: {sw.ElapsedMilliseconds} ms");

            return this;
        }

        private XMachine<ContextType> RegisterActionSubscriber(XMachineEffect<ContextType> eventEffectInstance)
        {
            var methods = eventEffectInstance
                .GetType()
                .GetMethods(bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(predicate: m =>
                    m.GetCustomAttribute<XMachineSubscribeAttribute>() != null &&
                    m.ReturnType == typeof(void) &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(XMachineAction))
                .ToList();

            foreach (MethodInfo method in methods)
            {
                XMachineSubscribeAttribute attr = method.GetCustomAttribute<XMachineSubscribeAttribute>();
                var subscribedTypes = attr.SubscribedTypes;

                // Tạo delegate runtime từ MethodInfo
                Action<XMachineAction> actionDelegate = action =>
                {
                    try
                    {
                        method.Invoke(obj: eventEffectInstance, parameters: new object[]
                        {
                            action
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(message: $"Error invoking handler '{method.Name}': {ex}");
                    }
                };

                RegisterSubscriber(actionSubscriber: actionDelegate, filterTypes: subscribedTypes);
            }

            return this;
        }

        public XMachine<ContextType> RegisterSubscriber(Action<XMachineAction> actionSubscriber, HashSet<string> filterTypes)
        {
            sharedActionStream
                .Where(predicate: action => action != XMachineAction.Empty && filterTypes.Contains(item: action.type))
                .Subscribe(
                    onNext: actionSubscriber,
                    onCompleted: HandleError
                )
                .AddTo(bag: ref disposable);

            return this;
        }


        public void SetContext(Func<ContextType, ContextType> contextUpdater)
        {
            reactiveContext.Value = contextUpdater(arg: reactiveContext.Value);
        }
    }

    public abstract class XMachineEffect<ContextType>
    {
        protected readonly XMachineActor<ContextType> actor;

        protected XMachineEffect(XMachineActor<ContextType> actor)
        {
            this.actor = actor;
        }

        protected ContextType contextValue
        {
            get
            {
                return actor.machine.GetContextValue();
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
                .RegisterEffects(actionEffectInstances: ConfigureMachineEffects());
        }

        protected virtual void Start()
        {
            machine.Start(initialStateId: ConfigureInitialStateId());
        }

        protected void Update()
        {
            machine.GetCurrentState().Update();
        }

        protected void FixedUpdate()
        {
            machine.GetCurrentState().FixedUpdate();
        }

        protected void OnDestroy()
        {
            machine.Stop();
        }

        protected void LogStateChange()
        {
            machine.reactiveCurrentStateId.Subscribe(onNext: state => Debug.Log(message: "State changed to: " + state));
        }

        protected abstract ContextType ConfigureInitialContext();
        protected abstract XMachineState<ContextType>[] ConfigureMachineStates();
        protected abstract XMachineEffect<ContextType>[] ConfigureMachineEffects();
        protected abstract Enum ConfigureInitialStateId();
    }
}
