using UnityEngine;
using UnityEngine.UIElements;
namespace ND25.UI
{
    public abstract class UIToolkitControllerAssetBase : ScriptableObject
    {
        public abstract void Setup(VisualElement root);
        public abstract void Cleanup();
    }
}
