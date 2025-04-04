using System;
using UnityEngine;
using XLua;

namespace Core.XLua
{
    [Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }

    public abstract class LuaMono : MonoBehaviour
    {
        private Action luaDestroy;
        private LuaTable luaModule;
        private Action luaStart;
        private Action luaUpdate;
        protected abstract string ModuleName { get; }

        protected virtual void Awake()
        {
            InitLuaEnv();
        }

        private void Start()
        {
            luaStart?.Invoke();
        }

        private void Update()
        {
            luaUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            luaDestroy?.Invoke();

            luaDestroy = null;
            luaUpdate = null;
            luaStart = null;

            if (luaModule != null) luaModule.Dispose();

            luaModule = null;
        }

        private void InitLuaEnv()
        {
            var luaEnv = LuaManager.GetInstance();

            var result = luaEnv.DoString($@"
                player = require(""{ModuleName}"")
                return player:new()
            ");

            if (result.Length != 1 || result[0] is not LuaTable)
            {
                Debug.LogError("Lua module not loaded properly!");
                return;
            }

            luaModule = result[0] as LuaTable;
            luaModule!.Set("monoBehaviour", this);

            // Lấy các function từ Lua
            luaModule.Get("Awake", out Action luaAwake);
            luaModule.Get("Start", out luaStart);
            luaModule.Get("Update", out luaUpdate);
            luaModule.Get("Destroy", out luaDestroy);

            // Gọi hàm awake nếu có
            luaAwake?.Invoke();
        }
    }
}