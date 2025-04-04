﻿using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using ND25.Character.Warrior;
using Newtonsoft.Json;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using DisposableBag = R3.DisposableBag;
namespace ND25.Core.ReactiveMachine
{
    public delegate Observable<ReactiveMachineAction> ReactiveMachineActionHandler(Observable<ReactiveMachineAction> upstream);

    public class ReactiveMachine<T>
    {
        public delegate Observable<Unit> ReactiveMachineContextHandler(Observable<T> upstream);

        readonly R3.Subject<ReactiveMachineAction> actionSubject = new R3.Subject<ReactiveMachineAction>();
        readonly T initialContext;
        readonly string jsonFileName;
        readonly R3.Observable<ReactiveMachineAction> sharedActionStream;

        readonly Dictionary<string, ReactiveMachineState<T>> states = new Dictionary<string, ReactiveMachineState<T>>();
        ReactiveMachineConfig config;


        DisposableBag disposable;
        public ReactiveMachine(T initialContext, string jsonFileName)
        {
            this.initialContext = initialContext;
            this.jsonFileName = jsonFileName;
            sharedActionStream = R3.ObservableExtensions.Share(actionSubject);
        }
        public ReactiveProperty<T> context { get; private set; }
        public ReactiveProperty<string> currentStateName { get; } = new ReactiveProperty<string>(null);

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
                var state = new ReactiveMachineState<T>(stateName, this, cfg);
                states[stateName] = state;

                // #if UNITY_EDITOR
                // Debug.Log($"[ReactiveMachine] Built state: {stateName}");
                // #endif
            }
        }

        void InitContext()
        {
            context = new R3.ReactiveProperty<T>(initialContext);
        }

        public void SetContext(Func<T, T> contextUpdater)
        {
            context.OnNext(contextUpdater(initialContext));
        }

        public void ContextChangeHandler(ReactiveMachineContextHandler eventHandler)
        {
            // Truyền trực tiếp subject vào handler để nhận stream xử lý
            eventHandler(context)
                .Subscribe(
                    (_) =>
                    {

                    },
                    error => Debug.LogError($"[ReactiveMachine] Error in event stream: {error}")
                )
                .AddTo(ref disposable);
        }

        public void DispatchEvent(Enum eventName)
        {
            // #if UNITY_EDITOR
            // Debug.Log($"[ReactiveMachine] Dispatch event: {eventName}");
            // #endif

            GetCurrentState()
                !.DispatchEvent(eventName.ToString());
        }

        public void DispatchAction(ReactiveMachineAction action)
        {
            actionSubject.OnNext(action);
        }

        [CanBeNull]
        ReactiveMachineState<T> GetCurrentState()
        {
            return currentStateName.Value == null ? null : states[currentStateName.Value];
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
            currentStateName.Value = nextState;
            GetCurrentState()!
                .Entry();
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
            InitContext();
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
            disposable.Dispose();
            actionSubject.Dispose();
        }
    }

}
