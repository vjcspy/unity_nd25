using System;
using UnityEngine;
using XLua;

namespace Core.XLua
{
    [CSharpCallLua]
    public interface ILuaMonoXState
    {
        // event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        Action Start();
        Action Update();
        Action OnDestroy();

        void Dispatch(string evenName, object[] args);
    }

    [CSharpCallLua]
    public delegate ILuaMonoXState LuaMonoXState();

    public abstract class MonoXState : MonoBehaviour
    {
        private LuaTable _luaModule;
        protected ILuaMonoXState _luaMonoXState;
        protected abstract string ModuleName { get; }

        protected virtual void Awake()
        {
            InitLuaEnv();
        }

        public void InitLuaEnv()
        {
            var luaEnv = LuaManager.GetInstance();

            var result = luaEnv.DoString(@$"
                xState = require(""{ModuleName}"")
                return xState:new()
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

            _luaModule.Get("LuaMonoXState", out LuaMonoXState LuaMonoXState);
            if (LuaMonoXState == null)
            {
                Debug.LogError("Lua module does not have an Awake method!");
                return;
            }

            _luaMonoXState = LuaMonoXState();
            if (_luaMonoXState == null)
            {
                Debug.LogError("Lua module does not return a valid LuaMonoXState!");
                return;
            }
        }

        private void Start()
        {
            _luaMonoXState?.Start();
        }

        private void Update()
        {
            _luaMonoXState?.Update();
        }

        private void OnDestroy()
        {
            _luaMonoXState?.OnDestroy();
            _luaModule?.Dispose();
            _luaMonoXState = null;
            _luaModule = null;
        }
    }
}