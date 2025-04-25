using Cysharp.Threading.Tasks;
using ND25.Core.Colyseus;
using UnityEngine;
using UnityEngine.UIElements;
// Import UniTask
namespace ND25.UI.InGame
{
    public class InGameController : MonoBehaviour
    {
        private Button myButton;

        private void OnEnable()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            myButton = root.Q<Button>(name: "MenuBtn");

            if (myButton != null)
            {
                myButton.clicked += OnMenuBtnClicked;
            }
        }

        private void OnDisable()
        {
            if (myButton != null)
            {
                myButton.clicked -= OnMenuBtnClicked;
            }
        }

        private async void OnMenuBtnClicked()
        {
            try
            {
                Debug.Log(message: "Button was clicked!");
                await DoSomethingAsync();
            }
            catch
            {
                // ignored
            }
        }

        private async UniTask DoSomethingAsync()
        {
            Debug.Log(message: "Starting async operation...");
            await ColyseusManager.Instance.SignInAsync(email: "test1@gmail.com", password: "test123456");
            Debug.Log(message: "Async operation finished!");
        }
    }
}
