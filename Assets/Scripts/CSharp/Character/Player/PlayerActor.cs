using System;
using CSharp.Core.XLua;
using UnityEngine;
using XLua;

namespace CSharp.Character.Player
{
    [Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }

    [LuaCallCSharp]
    public class PlayerActor : MonoBehaviour
    {
        public Injection[] injections;

        private LuaTable luaModule;

        private Action luaOnDestroy;
        private Action luaStart;
        private Action luaUpdate;

        private void Awake()
        {
            var luaEnv = LuaManager.GetInstance();

            // Gọi module Lua
            luaModule = luaEnv.DoString("return require('character.player')", "character.player")[0] as LuaTable;
            if (luaModule == null)
            {
                Debug.LogError("Lua module load failed!");
                return;
            }

            // Inject các biến C# vào Lua module
            luaModule.Set("self", this);
            foreach (var injection in injections)
            {
                luaModule.Set(injection.name, injection.value);
            }

            // Lấy các function từ Lua
            luaModule.Get("awake", out Action luaAwake);
            luaModule.Get("start", out luaStart);
            luaModule.Get("update", out luaUpdate);
            luaModule.Get("ondestroy", out luaOnDestroy);

            // Gọi hàm awake nếu có
            luaAwake?.Invoke();
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
            luaOnDestroy?.Invoke();

            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;

            luaModule?.Dispose();
            luaModule = null;

            injections = null;
        }
    }
}