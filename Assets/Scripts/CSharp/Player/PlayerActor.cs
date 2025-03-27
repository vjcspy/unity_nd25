using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
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

            // Add custom loader
            luaenv.AddLoader((ref string filename) =>
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    var url = "http://localhost:3000/load/" + filename;
                    StartCoroutine(LoadLuaScriptFromServer(url,
                        filename)); // Start Coroutine to load Lua script asynchronously
                }

                return null;
            });

            // Testing by loading Lua script from the server using the path 'my_script'
            luaenv.DoString("print('Loaded from server: ', require('lua_test'))");
        }

        private void OnDestroy()
        {
            luaenv.Dispose(); // Always dispose LuaEnv when done
        }

        // Coroutine to fetch Lua script from the server asynchronously using UnityWebRequest
        private IEnumerator LoadLuaScriptFromServer(string url, string filename)
        {
            // Create UnityWebRequest to fetch Lua script
            using (var request = UnityWebRequest.Get(url))
            {
                // Send the request and wait until it's done
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    // Lua script content is available in request.downloadHandler.text
                    var script = request.downloadHandler.text;

                    // Execute the Lua script in the Lua environment
                    luaenv.DoString(script);
                }
                else
                {
                    // Log status code and error message
                    var statusCode = request.responseCode; // Get HTTP status code
                    var errorMessage = request.error; // Get error message
                    var responseMessage = request.downloadHandler.text; // Get server response (if any)

                    // Log the status code and message
                    Debug.LogError(
                        $"Failed to load `{filename}` lua script from server. Status Code: {statusCode}. Error Message: {errorMessage}. Response Message: {responseMessage}");
                }
            }
        }
    }
}