using Colyseus;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
namespace ND25.Core.Colyseus
{
    public class ColyseusManager : MonoBehaviour
    {

        // [SerializeField]
        private readonly string serverEndpoint = "ws://localhost:2567";
        //
        // private readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();
        //
        // // Dùng ColyseusClient thay cho Client ở v0.16

        public ColyseusClient Client { get; private set; }

        // private Room<State> room;
        public static ColyseusManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(obj: gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(target: gameObject);

            // Khởi tạo ColyseusClient theo v0.16
            Client = new ColyseusClient(endpoint: serverEndpoint);
        }

        // Unity sẽ tự động gọi Start() và resume UniTask trên main thread
        private async UniTaskVoid Start()
        {
            await SignInAsync(email: "test1@gmail.com", password: "test123456");
        }

        public async UniTask SignInAsync(string email, string password)
        {
            ShowLoading(isLoading: true);

            try
            {
                // 1) Gọi SignIn… trả về Task<AuthUser>
                // 2) Chuyển thành UniTask<AuthUser>
                // 3) Gán cancellation token của GameObject
                IAuthData user = await Client.Auth
                    .SignInWithEmailAndPassword(email: email, password: password);

                Debug.Log(message: $"[Auth] Signed in: {user.RawUser}");
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning(message: "[Auth] SignIn was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError(message: $"[Auth] SignIn failed: {ex}");
            }
            finally
            {
                ShowLoading(isLoading: false);
            }
        }

        private void ShowLoading(bool isLoading)
        {
            // TODO: bật/tắt spinner hoặc overlay
            Debug.Log(message: $"Loading: {isLoading}");
        }
    }
}
