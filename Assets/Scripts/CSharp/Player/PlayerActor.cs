using UnityEngine;
using XLua;

namespace CSharp.Player
{
    public class PlayerActor : MonoBehaviour
    {
        private LuaEnv luaenv;

        // Use this for initialization
        private void Start()
        {
            luaenv = new LuaEnv();
            luaenv.DoString("require 'lua_test'");
        }

        // Update is called once per frame
        private void Update()
        {
            if (luaenv != null) luaenv.Tick();
        }

        private void OnDestroy()
        {
            luaenv.Dispose();
        }
    }
}