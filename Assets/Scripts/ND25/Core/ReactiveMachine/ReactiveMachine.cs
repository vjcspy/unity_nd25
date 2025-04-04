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
        readonly ReactiveMachineConfig config;

        string currentStateName;

        Dictionary<string, ReactiveMachineState> states;

        public ReactiveMachine(string jsonConfig)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ReactiveMachineStateConfigConverter());

            config = JsonConvert.DeserializeObject<ReactiveMachineConfig>(jsonConfig, settings);
        }

        void InitStates()
        {
            foreach (var kvp in config.states)
            {
                string stateName = kvp.Key;
                ReactiveMachineStateConfig cfg = kvp.Value;

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
            #if UNITY_EDITOR
            Debug.Log($"[ReactiveMachine] Dispatching Action: {action.type}");
            #endif
            actionSubject.OnNext(action);
        }

        [CanBeNull]
        ReactiveMachineState GetCurrentState()
        {
            return states[currentStateName];
        }

        public void TransitionTo(string nextState, List<ReactiveMachineAction> transitionActions)
        {
            #if UNITY_EDITOR
            Debug.Log($"[ReactiveMachine] Transition from {currentStateName} → {nextState}");
            #endif

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
                );
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
            InitStates();
        }

        public void Start()
        {
        }

        public void Update()
        {
            GetCurrentState()
                ?.Invoke();
        }

        public void OnDestroy()
        {
            actionSubject.Dispose();
        }
    }
}
