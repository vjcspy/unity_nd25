using System;
using UnityEngine;
using XLua;

namespace CSharp.Core.XLua
{
    [Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }

    public abstract class LuaMono : MonoBehaviour
    {
        private LuaTable luaModule;

        private Action luaOnDestroy;
        private Action luaStart;
        private Action luaUpdate;

        protected virtual void Awake()
        {
            InitLuaEnv();
        }

        protected void Start()
        {
            luaStart?.Invoke();
        }

        protected void Update()
        {
            luaUpdate?.Invoke();
        }

        protected void OnDestroy()
        {
            luaOnDestroy?.Invoke();

            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;

            luaModule?.Dispose();
            luaModule = null;
        }

        private void InitLuaEnv()
        {
            var luaEnv = LuaManager.GetNewEnv();

            var result = luaEnv.DoString("return require('character.player')", "character.player");
            if (result == null || result.Length == 0 || result[0] is not LuaTable)
            {
                Debug.LogError("Lua module not loaded properly!");
                return;
            }

            luaModule = result[0] as LuaTable;
            luaModule!.Set("monoBehaviour", this);

            // Lấy các function từ Lua
            luaModule.Get("awake", out Action luaAwake);
            luaModule.Get("start", out luaStart);
            luaModule.Get("update", out luaUpdate);
            luaModule.Get("ondestroy", out luaOnDestroy);

            // Gọi hàm awake nếu có
            luaAwake?.Invoke();
        }
    }
}