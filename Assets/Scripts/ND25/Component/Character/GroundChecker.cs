using System;
using UnityEngine;
namespace ND25.Component.Character
{
    internal enum CheckMethod
    {
        Raycast,
        OverlapCircle,
        // Có thể thêm BoxCast sau nếu muốn
    }

    public class GroundChecker : MonoBehaviour
    {

        [Header("Ground Check Settings")]
        [SerializeField]
        CheckMethod method = CheckMethod.Raycast;

        [SerializeField] Transform groundCheckPoint;
        [SerializeField] float checkDistance = 0.2f;
        [SerializeField] float overlapRadius = 0.25f;

        [SerializeField] LayerMask groundLayer;

        [Header("Debug")]
        [SerializeField]
        bool drawDebugRay = true;
        [SerializeField] bool drawGizmos = true;

        public bool isGrounded { get; private set; }

        void Awake()
        {
            InitializeGroundCheckPoint();
        }

        void Update()
        {
            switch (method)
            {
                case CheckMethod.Raycast:
                    CheckGroundRaycast();
                    break;
                case CheckMethod.OverlapCircle:
                    CheckGroundOverlap();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void OnDrawGizmos()
        {
            if (!drawGizmos || groundCheckPoint == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;

            switch (method)
            {
                case CheckMethod.Raycast:
                    Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * checkDistance);
                    Gizmos.DrawWireSphere(groundCheckPoint.position + Vector3.down * checkDistance, 0.05f);
                    break;
                case CheckMethod.OverlapCircle:
                    Gizmos.DrawWireSphere(groundCheckPoint.position, overlapRadius);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void InitializeGroundCheckPoint()
        {
            Transform found = transform.Find("GroundCheck");
            if (found != null)
            {
                groundCheckPoint = found;
            }
            else
            {
                GameObject emptyObj = new GameObject("GroundCheck")
                {
                    transform =
                    {
                        parent = transform,
                        localPosition = Vector3.zero,
                    },
                };
                groundCheckPoint = emptyObj.transform;
            }
        }

        void CheckGroundRaycast()
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, checkDistance, groundLayer);
            isGrounded = hit.collider;

            if (!drawDebugRay)
            {
                return;
            }

            Color rayColor = isGrounded ? Color.green : Color.red;
            Debug.DrawRay(groundCheckPoint.position, Vector2.down * checkDistance, rayColor);
        }

        void CheckGroundOverlap()
        {
            Collider2D hit = Physics2D.OverlapCircle(groundCheckPoint.position, overlapRadius, groundLayer);
            isGrounded = hit;

            if (!drawDebugRay)
            {
                return;
            }

            Color circleColor = isGrounded ? Color.green : Color.red;
            Debug.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * 0.01f, circleColor); // marker line
        }

        public void ForceCheck()
        {
            Update(); // gọi thủ công nếu cần
        }
    }
}
