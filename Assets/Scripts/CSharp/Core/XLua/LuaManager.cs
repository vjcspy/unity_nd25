using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

namespace CSharp.Core.XLua
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
            _luaEnv.AddLoader(LoadLuaScriptFromServer);

            return _luaEnv;
        }

        // This method returns a byte[] (script) and is passed to AddLoader
        private static byte[] LoadLuaScriptFromServer(ref string filename)
        {
            // Construct the URL to fetch the Lua script
            var url = "http://localhost:3000/load/" + filename;

            // Use UnityWebRequest to fetch the Lua script (blocking or async)
            using var request = UnityWebRequest.Get(url);
            // Wait for the request to complete
            request.SendWebRequest();

            // Check if the request was successful
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Return the script as a byte array
                Debug.Log("Loaded lua script from server: " + filename);
                return Encoding.UTF8.GetBytes(request.downloadHandler.text);
            }

            // Log the error if the request fails
            Debug.LogError($"Failed to load `{filename}` lua script from server. Error: {request.error}");
            return null;
        }
    }
}