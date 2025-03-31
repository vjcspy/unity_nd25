using UnityEngine;

namespace Character
{
    public class GroundChecker : MonoBehaviour
    {
        public enum CheckMethod
        {
            Raycast,
            OverlapCircle
            // Có thể thêm BoxCast sau nếu muốn
        }

        [Header("Ground Check Settings")]
        [SerializeField] private CheckMethod method = CheckMethod.Raycast;

        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float checkDistance = 0.2f;
        [SerializeField] private float overlapRadius = 0.25f;

        [SerializeField] private LayerMask groundLayer;

        [Header("Debug")]
        [SerializeField] private bool drawDebugRay = true;
        [SerializeField] private bool drawGizmos = true;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            // Nếu groundCheckPoint chưa được thiết lập, tự động tìm hoặc tạo một child GameObject có tên "GroundCheck"
            if (groundCheckPoint == null)
            {
                Transform found = transform.Find("GroundCheck");
                if (found != null)
                {
                    groundCheckPoint = found;
                }
                else
                {
                    GameObject emptyObj = new("GroundCheck");
                    emptyObj.transform.parent = transform;
                    emptyObj.transform.localPosition = Vector3.zero;
                    groundCheckPoint = emptyObj.transform;
                }
            }
        }

        private void Update()
        {
            switch (method)
            {
                case CheckMethod.Raycast:
                    CheckGroundRaycast();
                    break;
                case CheckMethod.OverlapCircle:
                    CheckGroundOverlap();
                    break;
            }
        }

        private void CheckGroundRaycast()
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, checkDistance, groundLayer);
            IsGrounded = hit.collider != null;

            if (drawDebugRay)
            {
                Color rayColor = IsGrounded ? Color.green : Color.red;
                Debug.DrawRay(groundCheckPoint.position, Vector2.down * checkDistance, rayColor);
            }
        }

        private void CheckGroundOverlap()
        {
            Collider2D hit = Physics2D.OverlapCircle(groundCheckPoint.position, overlapRadius, groundLayer);
            IsGrounded = hit != null;

            if (drawDebugRay)
            {
                Color circleColor = IsGrounded ? Color.green : Color.red;
                Debug.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * 0.01f, circleColor); // marker line
            }
        }

        public void ForceCheck()
        {
            Update(); // gọi thủ công nếu cần
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos || groundCheckPoint == null) return;

            Gizmos.color = Color.yellow;

            if (method == CheckMethod.Raycast)
            {
                Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * checkDistance);
                Gizmos.DrawWireSphere(groundCheckPoint.position + Vector3.down * checkDistance, 0.05f);
            }
            else if (method == CheckMethod.OverlapCircle)
            {
                Gizmos.DrawWireSphere(groundCheckPoint.position, overlapRadius);
            }
        }
    }
}