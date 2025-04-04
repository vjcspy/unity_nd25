using System;
using UnityEngine;
using XLua;
namespace ND25.Core.XLua
{
    [CSharpCallLua]
    public interface ILuaStateMachineMono
    {
        // event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        Action Start();
        Action Update();
        Action OnDestroy();

        void Dispatch(string evenName, object[] args = null);
        void Set(string key, object value);
        void Initialize();
    }

    [CSharpCallLua]
    public delegate ILuaStateMachineMono LuaStateMachineMono();




    public abstract class StateMachineActorMono : MonoBehaviour
    {
        private LuaTable luaModule;
        protected ILuaStateMachineMono luaStateMachineMono;
        protected abstract string ModuleName { get; }

        protected virtual void Awake()
        {
            InitLuaEnv();
        }

        public void InitLuaEnv()
        {
            var luaEnv = LuaManager.GetInstance();

            var result = luaEnv.DoString(@$"
                warrior = require(""{ModuleName}"")
                return warrior:factory()
            ");

            if (result.Length != 1 || result[0] is not LuaTable)
            {
                Debug.LogError("Lua module not loaded properly!");
                return;
            }

            if (result[0] is not LuaTable _luaModule)
            {
                Debug.LogError("Lua module is null!");
                return;
            }
            LuaStateMachineMono LuaStateMachineMono = _luaModule.Get<LuaStateMachineMono>("LuaStateMachineMono");

            if (LuaStateMachineMono == null)
            {
                Debug.LogError("Lua module does not have an LuaStateMachineMono method!");
                return;
            }

            luaStateMachineMono = LuaStateMachineMono();
            if (luaStateMachineMono == null)
            {
                Debug.LogError("Lua module does not return a valid LuaStateMachineMono");
                return;
            }
            luaStateMachineMono.Set("monoBehaviourCSharp", this);
        }

        protected virtual void Start()
        {
            luaStateMachineMono.Start();
        }

        protected virtual void Update()
        {
            luaStateMachineMono.Update();
        }

        private void OnDestroy()
        {
            luaStateMachineMono?.OnDestroy();
            luaModule?.Dispose();
            luaStateMachineMono = null;
            luaModule = null;
        }
    }
}