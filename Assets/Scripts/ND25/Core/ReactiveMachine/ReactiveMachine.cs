using JetBrains.Annotations;
using Newtonsoft.Json;
using R3;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace ND25.Core.ReactiveMachine
{
    public delegate Observable<ReactiveMachineAction> ReactiveMachineActionHandler(
        Observable<ReactiveMachineAction> upstream
    );

    public class ReactiveMachine
    {
        readonly BehaviorSubject<ReactiveMachineAction> actionSubject = new BehaviorSubject<ReactiveMachineAction>(null);
        readonly string jsonFileName;

        readonly Dictionary<string, ReactiveMachineState> states = new Dictionary<string, ReactiveMachineState>();
        ReactiveMachineConfig config;

        string currentStateName;

        DisposableBag disposable;

        public ReactiveMachine(string jsonFileName)
        {
            this.jsonFileName = jsonFileName;
        }

        void LoadConfig()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>($"Configs/ReactiveStateMachine/{jsonFileName}.json");
            string jsonContent = jsonFile.text;
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ReactiveMachineStateConfigConverter());

            config = JsonConvert.DeserializeObject<ReactiveMachineConfig>(jsonContent, settings);
        }

        void InitStates()
        {
            foreach ((string stateName, ReactiveMachineStateConfig cfg) in config.states)
            {

                ReactiveMachineState state = new ReactiveMachineState(this, cfg);
                states[stateName] = state;

                #if UNITY_EDITOR
                Debug.Log($"[ReactiveMachine] Built state: {stateName}");
                #endif
            }
        }

        public void DispatchEvent(string eventName)
        {
            GetCurrentState()
                ?.DispatchEvent(eventName);
        }

        public void DispatchAction(ReactiveMachineAction action)
        {
            actionSubject.OnNext(action);
        }

        [CanBeNull]
        ReactiveMachineState GetCurrentState()
        {
            return states[currentStateName];
        }

        public void TransitionTo(string nextState, [CanBeNull] List<ReactiveMachineAction> transitionActions = null)
        {
            // Exit current state
            GetCurrentState()
                ?.Exit();

            // Run transition-specific actions if any
            if (transitionActions != null)
            {
                foreach (ReactiveMachineAction action in transitionActions)
                {
                    DispatchAction(action);
                }
            }

            // Update current state and enter new one
            currentStateName = nextState;
            GetCurrentState()
                ?.Entry();
        }

        void RegisterAction(ReactiveMachineActionHandler eventHandler)
        {
            actionSubject
                .SelectMany(
                    originalEvent =>
                        eventHandler(Observable.Return(originalEvent))
                            .Select(handledEvent => new { Original = originalEvent, Handled = handledEvent, })
                )
                .Where(events => events.Handled != ReactiveMachineCoreAction.Empty)
                .Subscribe(
                    events =>
                    {
                        ReactiveMachineAction handledEvent = events.Handled;
                        DispatchAction(handledEvent);
                    },
                    error => Debug.LogError($"Error in event stream: {error.ToString()}")
                ).AddTo(ref disposable);
        }

        void RegisterActionHandler(object eventEffectInstance)
        {
            var methods = eventEffectInstance
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ReactiveMachineEffectAttribute>() != null && m.ReturnType == typeof(ReactiveMachineActionHandler))
                .ToList();

            foreach (ReactiveMachineActionHandler eventHandler in methods.Select(
                method =>
                    (ReactiveMachineActionHandler)method.Invoke(eventEffectInstance, null)!
            ))
            {
                RegisterAction(eventHandler);
            }
        }

        public void RegisterActionHandler(object[] actionEffectInstances)
        {
            foreach (object eventEffectInstance in actionEffectInstances)
            {
                RegisterActionHandler(eventEffectInstance);
            }
        }

        public void Awake()
        {
            LoadConfig();
            // InitStates();
        }

        public void Start()
        {
            TransitionTo(config.initial);
        }

        public void Update()
        {
            GetCurrentState()!.Invoke();
        }

        public void OnDestroy()
        {
            actionSubject.Dispose();
            disposable.Dispose();
        }
    }


    public abstract class ReactiveMachineMono : MonoBehaviour
    {
        ReactiveMachine machine;

        protected virtual void Awake()
        {
            machine = new ReactiveMachine(GetJsonFileName());
            machine.Awake();
            // machine.RegisterActionHandler(GetActionHandlers());
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
