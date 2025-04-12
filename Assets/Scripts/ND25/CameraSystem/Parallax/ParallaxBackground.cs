using UnityEngine;
namespace ND25.CameraSystem.Parallax
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [Tooltip("Tốc độ parallax của layer này (0 = đứng yên, 1 = theo sát camera).")]
        [Range(0f, 1f)]
        public float parallaxMultiplier = 0.5f;

        [Tooltip("Có nên áp dụng hiệu ứng theo trục Y không?")]
        public bool affectY;

        [Header("Camera Settings")]
        [Tooltip("Camera tham chiếu. Nếu để trống sẽ tự động dùng Camera.main.")]
        public UnityEngine.Camera referenceCamera;

        Transform camTransform;
        bool isInitialized;
        Vector3 lastCamPosition;

        void Start()
        {
            InitCamera();
            if (camTransform == null) return;
            lastCamPosition = camTransform.position;
            isInitialized = true;
        }

        void LateUpdate()
        {
            if (!isInitialized) return;

            Vector3 deltaMovement = camTransform.position - lastCamPosition;

            // Tính toán offset dựa trên multiplier
            float moveX = deltaMovement.x * parallaxMultiplier;
            float moveY = affectY ? deltaMovement.y * parallaxMultiplier : 0f;

            transform.position += new Vector3(moveX, moveY, 0);

            lastCamPosition = camTransform.position;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            // Đảm bảo multiplier luôn nằm trong khoảng cho phép
            parallaxMultiplier = Mathf.Clamp01(parallaxMultiplier);
        }
#endif

        void InitCamera()
        {
            if (referenceCamera != null)
            {
                camTransform = referenceCamera.transform;
            }
            else if (UnityEngine.Camera.main != null)
            {
                camTransform = UnityEngine.Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("[ParallaxLayer] Không tìm thấy camera.");
                camTransform = null;
            }
        }
    }
}
