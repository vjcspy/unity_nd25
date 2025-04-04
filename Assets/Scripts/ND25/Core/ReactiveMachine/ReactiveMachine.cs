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
        readonly Subject<ReactiveMachineAction> actionSubject = new Subject<ReactiveMachineAction>();
        readonly string jsonFileName;
        readonly Observable<ReactiveMachineAction> sharedActionStream;

        readonly Dictionary<string, ReactiveMachineState> states = new Dictionary<string, ReactiveMachineState>();
        ReactiveMachineConfig config;

        [CanBeNull] string currentStateName;

        DisposableBag disposable;

        public ReactiveMachine(string jsonFileName)
        {
            this.jsonFileName = jsonFileName;
            sharedActionStream = actionSubject.Share();
        }

        void LoadConfig()
        {
            string filePath = $"Configs/ReactiveStateMachine/{jsonFileName}";
            TextAsset jsonFile = Resources.Load<TextAsset>(filePath);
            if (jsonFile == null)
            {
                Debug.LogError($"Failed to load JSON file at path: {filePath}");
                return;
            }

            string jsonContent = jsonFile.text;
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ReactiveMachineStateConfigConverter());

            config = JsonConvert.DeserializeObject<ReactiveMachineConfig>(jsonContent, settings);
        }

        void InitStates()
        {
            foreach ((string stateName, ReactiveMachineStateConfig cfg) in config.states)
            {
                ReactiveMachineState state = new ReactiveMachineState(stateName, this, cfg);
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
            return currentStateName == null ? null : states[currentStateName];
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
            // Truyền trực tiếp subject vào handler để nhận stream xử lý
            eventHandler(sharedActionStream)
                .Where(handledEvent => handledEvent != ReactiveMachineCoreAction.Empty)
                .Subscribe(
                    DispatchAction,
                    error => Debug.LogError($"[ReactiveMachine] Error in event stream: {error}")
                )
                .AddTo(ref disposable);
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
            InitStates();
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

}
