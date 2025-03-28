using CSharp.Core.XLua;
using UnityEngine;
using XLua;

namespace CSharp.Character.Player
{
    public class PlayerActor : MonoBehaviour
    {
        private  LuaEnv luaenv;

        // Use this for initialization
        private void Start()
        {
            luaenv = LuaManager.GetInstance();
            // Testing by loading Lua script from the server using the path 'my_script'
            luaenv.DoString("print('Loaded from server: ', require('lua_test'))");
        }
    }
}