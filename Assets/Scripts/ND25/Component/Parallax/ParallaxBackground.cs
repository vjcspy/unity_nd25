using UnityEngine;
namespace ND25.Component.Parallax
{

    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class ParallaxLayer : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [Tooltip("Tốc độ parallax của layer này (0 = đứng yên, 1 = theo sát camera).")]
        [Range(0f, 1f)]
        public float parallaxMultiplier = 0.5f;

        [Tooltip("Có nên áp dụng hiệu ứng theo trục Y không?")]
        public bool affectY;

        [Header("Camera Settings")]
        [Tooltip("Camera tham chiếu. Nếu để trống sẽ tự động dùng Camera.main.")]
        public Camera referenceCamera;

        Transform camTransform;
        Vector3 lastCamPosition;


        void Start()
        {
            InitCamera();

            if (camTransform != null)
            {
                lastCamPosition = camTransform.position;
            }
        }

        void Update()
        {
            if (!camTransform) return;

            Vector3 deltaMovement = camTransform.position - lastCamPosition;

            float moveX = deltaMovement.x * parallaxMultiplier;
            float moveY = affectY ? deltaMovement.y * parallaxMultiplier : 0f;

            transform.position += new Vector3(moveX, moveY, 0);
            lastCamPosition = camTransform.position;
        }


        void InitCamera()
        {
            if (referenceCamera != null)
            {
                camTransform = referenceCamera.transform;
            }
            else if (Camera.main != null)
            {
                camTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("[ParallaxLayer] Could not find a camera. Please assign one in the inspector or ensure there is a Camera in the scene.");
                camTransform = null;
            }
        }
    }
}
