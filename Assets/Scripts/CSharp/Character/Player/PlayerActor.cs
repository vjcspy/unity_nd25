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

    public class PlayerActor : MonoBehaviour
    {
        private  LuaEnv luaEnv;


        private Action luaStart;
        private Action luaUpdate;
        private Action luaOnDestroy;

        private LuaTable scriptScopeTable;

        public Injection[] injections;

        private void Awake()
        {
            luaEnv = LuaManager.GetInstance();

            scriptScopeTable = luaEnv.NewTable();
            using (var meta = luaEnv.NewTable())
            {
                meta.Set("__index", luaEnv.Global);
                scriptScopeTable.SetMetaTable(meta);
            }

            // 将所需值注入到 Lua 脚本域中
            scriptScopeTable.Set("self", this);
            foreach (var injection in injections)
            {
                scriptScopeTable.Set(injection.name, injection.value);
            }

            luaEnv.DoString("require('player')");

            var luaAwake = scriptScopeTable.Get<Action>("awake");
            scriptScopeTable.Get("start", out luaStart);
            scriptScopeTable.Get("update", out luaUpdate);
            scriptScopeTable.Get("ondestroy", out luaOnDestroy);

            luaAwake?.Invoke();
        }

        void Start()
        {
            luaStart?.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            luaUpdate?.Invoke();
        }

        void OnDestroy()
        {
            luaOnDestroy?.Invoke();

            scriptScopeTable.Dispose();
            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            injections = null;
        }
    }
}