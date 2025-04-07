using UnityEngine;
namespace ND25.Component.Sprite
{
    /// <summary>
    ///     SpriteCloneRepeater là một MonoBehaviour dùng để tạo ra các bản sao của sprite trong Unity.
    ///     Work with SpriteRenderer just clone and doesn't change any property.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteCloneRepeater : MonoBehaviour
    {
        Transform camTransform;

        SpriteRenderer mainRenderer;

        float spriteWidth;

        void Awake()
        {
            mainRenderer = GetComponent<SpriteRenderer>();
            if (!mainRenderer || !mainRenderer.sprite)
            {
                Debug.LogError("[ParallaxRepeater] SpriteRenderer or sprite missing.");
                enabled = false;
                return;
            }

            UnityEngine.Sprite sprite = mainRenderer.sprite;

            float unitWidth = sprite.rect.width / sprite.pixelsPerUnit;
            spriteWidth = unitWidth * transform.lossyScale.x; // Tính chính xác theo scale hiện tại
        }

        void Start()
        {
            if (Camera.main != null)
            {
                camTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("[SpriteCloneRepeater] No main camera found.");
                enabled = false;
                return;
            }

            CreateCopies();
        }

        void Update()
        {
            HandleLooping();
        }

        void CreateCopies()
        {
            // Left Copy
            InstantiateCopy("Left Copy", -spriteWidth);

            // Right Copy
            InstantiateCopy("Right Copy", +spriteWidth);
        }

        void InstantiateCopy(string objectName, float offsetX)
        {
            GameObject copy = new GameObject(objectName);
            copy.transform.SetParent(transform);

            SpriteRenderer sr = copy.AddComponent<SpriteRenderer>();

            sr.sprite = mainRenderer.sprite;
            sr.sortingLayerID = mainRenderer.sortingLayerID;
            sr.sortingOrder = mainRenderer.sortingOrder;

            copy.transform.localPosition = new Vector3(offsetX, 0f, 0f);

        }

        void HandleLooping()
        {
            if (!camTransform) return;

            float camX = camTransform.position.x;
            float objX = transform.position.x;

            float distanceFromCam = camX - objX;

            if (!(Mathf.Abs(distanceFromCam) >= spriteWidth)) return;
            int direction = distanceFromCam > 0 ? 1 : -1;
            transform.position += new Vector3(spriteWidth * direction, 0f, 0f);
        }
    }
}
