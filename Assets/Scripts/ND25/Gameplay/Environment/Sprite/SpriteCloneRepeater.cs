using UnityEngine;
namespace ND25.Gameplay.Environment.Sprite
{
    /// <summary>
    ///     SpriteCloneRepeater là một MonoBehaviour dùng để tạo ra các bản sao của sprite trong Unity.
    ///     Work with SpriteRenderer just clone and doesn't change any property.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(requiredComponent: typeof(SpriteRenderer))]
    public class SpriteCloneRepeater : MonoBehaviour
    {
        private Transform camTransform;

        private SpriteRenderer mainRenderer;

        private float spriteWidth;

        private void Awake()
        {
            mainRenderer = GetComponent<SpriteRenderer>();
            if (!mainRenderer || !mainRenderer.sprite)
            {
                Debug.LogError(message: "[ParallaxRepeater] SpriteRenderer or sprite missing.");
                enabled = false;
                return;
            }

            UnityEngine.Sprite sprite = mainRenderer.sprite;

            float unitWidth = sprite.rect.width / sprite.pixelsPerUnit;
            spriteWidth = unitWidth * transform.lossyScale.x; // Tính chính xác theo scale hiện tại
        }

        private void Start()
        {
            if (Camera.main != null)
            {
                camTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(message: "[SpriteCloneRepeater] No main camera found.");
                enabled = false;
                return;
            }

            CreateCopies();
        }

        private void Update()
        {
            HandleLooping();
        }

        private void CreateCopies()
        {
            // Left Copy
            InstantiateCopy(objectName: "Left Copy", offsetX: -spriteWidth);

            // Right Copy
            InstantiateCopy(objectName: "Right Copy", offsetX: +spriteWidth);
        }

        private void InstantiateCopy(string objectName, float offsetX)
        {
            GameObject copy = new GameObject(name: objectName);
            copy.transform.SetParent(p: transform);

            SpriteRenderer sr = copy.AddComponent<SpriteRenderer>();

            sr.sprite = mainRenderer.sprite;
            sr.sortingLayerID = mainRenderer.sortingLayerID;
            sr.sortingOrder = mainRenderer.sortingOrder;

            copy.transform.localPosition = new Vector3(x: offsetX, y: 0f, z: 0f);

        }

        private void HandleLooping()
        {
            if (!camTransform) return;

            float camX = camTransform.position.x;
            float objX = transform.position.x;

            float distanceFromCam = camX - objX;

            if (!(Mathf.Abs(f: distanceFromCam) >= spriteWidth)) return;
            int direction = distanceFromCam > 0 ? 1 : -1;
            transform.position += new Vector3(x: spriteWidth * direction, y: 0f, z: 0f);
        }
    }
}
