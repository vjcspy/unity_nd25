﻿using System.IO;
using System.Text;
using UnityEngine;
using XLua;

namespace Core.XLua
{
    public class LuaManager : MonoBehaviour
    {
        private const float GCInterval = 1f;
        private static LuaEnv _luaEnv;
        private static float _lastGCTime;

        private void Awake()
        {
            GetInstance();
        }

        private void Update()
        {
            if (!(Time.time - _lastGCTime > GCInterval)) return;

            _luaEnv.Tick();
            _lastGCTime = Time.time;
        }

        private void OnDestroy()
        {
            _luaEnv.Dispose(); // Always dispose LuaEnv when done
        }

        public static LuaEnv GetInstance()
        {
            if (_luaEnv != null) return _luaEnv;

            Debug.Log("Creating new LuaEnv instance");
            _luaEnv = new LuaEnv();

            // AddLoader expects a delegate that returns byte[]
            _luaEnv.AddLoader(LoadLuaScriptDevelopment);

            return _luaEnv;
        }

        public static LuaEnv GetNewEnv()
        {
            Debug.Log("Creating new LuaEnv instance");
            var luaEnv = new LuaEnv();

            // AddLoader expects a delegate that returns byte[]
            luaEnv.AddLoader(LoadLuaScriptDevelopment);

            return luaEnv;
        }

        // This method returns a byte[] (script) and is passed to AddLoader
        private static byte[] LoadLuaScriptDevelopment(ref string filename)
        {
            var filepath = Application.dataPath + "/Scripts/XLua/" + filename.Replace('.', '/') + ".lua";
            // Debug.Log($"Attempting to load Lua script from: {filepath}");

            if (!File.Exists(filepath))
            {
                Debug.LogError("File does not exist");
                return null;
            }
            using var reader = new StreamReader(filepath, Encoding.UTF8);
            var script = reader.ReadToEnd();

            // Debug.Log($"Loaded Lua script from: {filepath}");
            return Encoding.UTF8.GetBytes(script);
        }
    }
}