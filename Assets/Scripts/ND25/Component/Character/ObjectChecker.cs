using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ND25.Component.Character
{
    internal enum CheckMethod
    {
        Raycast,
        OverlapCircle
    }

    [ExecuteAlways]
    public class ObjectChecker : MonoBehaviour
    {
        [Header("Ground Check Settings")]
        [SerializeField]
        CheckMethod groundCheckMethod = CheckMethod.Raycast;
        [SerializeField] Transform groundCheckPoint;
        [SerializeField] float checkDistance = 0.2f;
        [SerializeField] float overlapRadius = 0.25f;
        [SerializeField] LayerMask groundLayer;

        [Header("Side Obstacle Check Settings")]
        [SerializeField]
        float sideObstacleDistance = 0.5f;
        [SerializeField] LayerMask obstacleLayer;

        [Header("Player Detection Settings")]
        [SerializeField]
        CheckMethod playerCheckMethod = CheckMethod.Raycast;
        [SerializeField] float playerDetectDistance = 1f;
        [SerializeField] LayerMask playerLayer;

        [Header("Debug")]
        [SerializeField]
        bool drawDebugRay;
        [SerializeField] bool drawGizmos = true;

        readonly Collider2D[] playerDetectionBufferLeft = new Collider2D[4];
        readonly Collider2D[] playerDetectionBufferRight = new Collider2D[4];

        public bool isGrounded { get; private set; }
        public bool isBlockedLeft { get; private set; }
        public bool isBlockedRight { get; private set; }
        public bool hasPlayerLeft { get; private set; }
        public bool hasPlayerRight { get; private set; }

        #region Gizmos Visualization

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!drawGizmos || groundCheckPoint == null) return;

            // GROUND CHECK GIZMO
            Gizmos.color = Color.yellow;
            switch (groundCheckMethod)
            {
                case CheckMethod.Raycast:
                    Gizmos.DrawLine(groundCheckPoint.position,
                        groundCheckPoint.position + Vector3.down * checkDistance);
                    break;

                case CheckMethod.OverlapCircle:
                    Gizmos.DrawWireSphere(groundCheckPoint.position, overlapRadius);
                    break;
            }

            // OBSTACLE CHECKS - RED LINES LEFT/RIGHT
            if (obstacleLayer != LayerMask.NameToLayer("Default"))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(groundCheckPoint.position,
                    groundCheckPoint.position + Vector3.left * sideObstacleDistance);

                Gizmos.DrawLine(groundCheckPoint.position,
                    groundCheckPoint.position + Vector3.right * sideObstacleDistance);
            }



            // PLAYER CHECKS - BLUE LINES LEFT/RIGHT OR SPHERES IF OVERLAP MODE.
            Gizmos.color = Color.blue;

            Vector3 upOffsetPos = groundCheckPoint.position + Vector3.up * 0.05f;

            if (playerLayer != LayerMask.NameToLayer("Default"))
            {
                switch (playerCheckMethod)
                {
                    case CheckMethod.Raycast:
                        Gizmos.DrawLine(upOffsetPos,
                            upOffsetPos + Vector3.left * playerDetectDistance);

                        Gizmos.DrawLine(upOffsetPos,
                            upOffsetPos + Vector3.right * playerDetectDistance);
                        break;


                    case CheckMethod.OverlapCircle:
                        Gizmos.DrawWireSphere(upOffsetPos +
                            Vector3.left * playerDetectDistance * 0.5f,
                            overlapRadius);

                        Gizmos.DrawWireSphere(upOffsetPos +
                            Vector3.right * playerDetectDistance * 0.5f,
                            overlapRadius);
                        break;
                }
            }
        }
#endif

        #endregion

        #region Initialization

        void InitializeGroundCheckPoint()
        {
            if (groundCheckPoint != null) return;

            Transform found = transform.Find("ObjectChecker");
            if (found != null)
            {
                groundCheckPoint = found;
            }
            else
            {
#if UNITY_EDITOR
                GameObject obj = new GameObject("ObjectChecker");
                Undo.RegisterCreatedObjectUndo(obj, "Create ObjectChecker");
#else
       GameObject obj = new GameObject("ObjectChecker");
#endif
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                groundCheckPoint = obj.transform;

#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        #endregion

        #region Ground Checking

        void CheckGround()
        {
            switch (groundCheckMethod)
            {
                case CheckMethod.Raycast:
                    RaycastHit2D hit =
                        Physics2D.Raycast(groundCheckPoint.position,
                            Vector2.down,
                            checkDistance,
                            groundLayer);

                    isGrounded = hit.collider;

                    if (drawDebugRay)
                    {
                        Color rayColor = isGrounded ? Color.green : Color.red;
                        Debug.DrawRay(groundCheckPoint.position,
                            Vector2.down * checkDistance,
                            rayColor);
                    }
                    break;

                case CheckMethod.OverlapCircle:
                    Collider2D overlap =
                        Physics2D.OverlapCircle(groundCheckPoint.position,
                            overlapRadius,
                            groundLayer);

                    isGrounded = overlap;

                    if (drawDebugRay)
                    {
                        Color color = isGrounded ? Color.green : Color.red;
                        Debug.DrawLine(groundCheckPoint.position,
                            groundCheckPoint.position + Vector3.down * 0.01f,
                            color);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Obstacle Checking

        void CheckObstacles()
        {
            Vector3 origin = groundCheckPoint.position;

            RaycastHit2D leftHit =
                Physics2D.Raycast(origin, Vector2.left, sideObstacleDistance, obstacleLayer);

            RaycastHit2D rightHit =
                Physics2D.Raycast(origin, Vector2.right, sideObstacleDistance, obstacleLayer);

            isBlockedLeft = leftHit.collider;
            isBlockedRight = rightHit.collider;

#if UNITY_EDITOR
            if (!drawDebugRay) return;

            Debug.DrawRay(origin,
                Vector2.left * sideObstacleDistance,
                isBlockedLeft ? Color.red : new Color(1f, .5f, .5f));

            Debug.DrawRay(origin,
                Vector2.right * sideObstacleDistance,
                isBlockedRight ? Color.red : new Color(1f, .5f, .5f));
#endif
        }

        #endregion

        #region Player Checking

        void CheckPlayers()
        {
            Vector3 originL = groundCheckPoint.position + Vector3.up * 0.05f;
            Vector3 originR = originL;

            switch (playerCheckMethod)
            {
                case CheckMethod.Raycast:
                    RaycastHit2D hitL =
                        Physics2D.Raycast(originL, Vector2.left, playerDetectDistance, playerLayer);

                    RaycastHit2D hitR =
                        Physics2D.Raycast(originR, Vector2.right, playerDetectDistance, playerLayer);

                    hasPlayerLeft = hitL.collider;
                    hasPlayerRight = hitR.collider;

                    if (drawDebugRay)
                    {
                        Debug.DrawRay(originL,
                            Vector2.left * playerDetectDistance,
                            hasPlayerLeft ? Color.blue : new Color(0.6f, 0.6f, .9f));

                        Debug.DrawRay(originR,
                            Vector2.right * playerDetectDistance,
                            hasPlayerRight ? Color.blue : new Color(0.6f, .6f, .9f));
                    }

                    break;


                case CheckMethod.OverlapCircle:
                    Array.Clear(playerDetectionBufferRight, 0, playerDetectionBufferRight.Length);

                    ContactFilter2D contactFilter = new ContactFilter2D();
                    contactFilter.SetLayerMask(playerLayer);

                    int countLeft =
                        Physics2D.OverlapCircle(
                            originL + Vector3.left * (playerDetectDistance * 0.5f),
                            overlapRadius,
                            contactFilter,
                            playerDetectionBufferLeft);

                    int countRight =
                        Physics2D.OverlapCircle(
                            originR + Vector3.right * (playerDetectDistance * 0.5f),
                            overlapRadius,
                            contactFilter,
                            playerDetectionBufferRight);

                    hasPlayerLeft = countLeft > 0;
                    hasPlayerRight = countRight > 0;

#if UNITY_EDITOR
                    if (drawDebugRay)
                    {
                        Debug.DrawLine(originL,
                            originL + Vector3.left * playerDetectDistance,
                            hasPlayerLeft ? Color.cyan : new Color(.4f, .4f, .8f));

                        Debug.DrawLine(originR,
                            originR + Vector3.right * playerDetectDistance,
                            hasPlayerRight ? Color.cyan : new Color(.4f, .4f, .8f));
                    }
#endif
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion


        #region Public API for Lua or AI system usage.

        public void ForceUpdateAllChecks()
        {
            Update(); // Có thể gọi từ Lua hoặc hệ thống AI khi cần cập nhật thủ công.
        }

        #endregion

        #region Unity Methods

        void Awake()
        {
            // InitializeGroundCheckPoint();
        }

#if UNITY_EDITOR
        void Reset()
        {
            InitializeGroundCheckPoint();
            EditorUtility.SetDirty(this);
        }
#endif

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!Application.isPlaying)
            {
                InitializeGroundCheckPoint();
            }
        }
#endif

        void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif

            CheckGround();
            CheckObstacles();
            CheckPlayers();
        }

        #endregion

    }
}
