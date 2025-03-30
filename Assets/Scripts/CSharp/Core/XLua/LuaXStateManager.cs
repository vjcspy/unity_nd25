using System;
using UnityEngine;
using XLua;

namespace Core.XLua
{
    [CSharpCallLua]
    public interface ILuaStateMachineMono
    {
        // event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        Action Start();
        Action Update();
        Action OnDestroy();

        void Dispatch(string evenName, object[] args);
        void SetData(string key, object value);
    }

    [CSharpCallLua]
    public delegate ILuaStateMachineMono LuaStateMachineMono();


    public abstract class XStateMonoBehavior : MonoBehaviour
    {
        private LuaTable _luaModule;
        protected ILuaStateMachineMono _luaStateMachineMono;
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

            _luaStateMachineMono = LuaStateMachineMono();
            if (_luaStateMachineMono == null)
            {
                Debug.LogError("Lua module does not return a valid LuaStateMachineMono");
                return;
            }
            _luaStateMachineMono.SetData("monoBehaviourCSharp", this);
        }

        private void Start()
        {
            _luaStateMachineMono?.Start();
        }

        private void Update()
        {
            _luaStateMachineMono?.Update();
        }

        private void OnDestroy()
        {
            _luaStateMachineMono?.OnDestroy();
            _luaModule?.Dispose();
            _luaStateMachineMono = null;
            _luaModule = null;
        }
    }
}