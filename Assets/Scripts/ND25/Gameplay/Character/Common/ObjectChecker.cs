using System;
using UnityEditor;
using UnityEngine;
namespace ND25.Gameplay.Character.Common
{
    internal enum CheckMethod
    {
        Raycast,
        OverlapCircle
    }

    [ExecuteAlways]
    public class ObjectChecker : MonoBehaviour
    {
        [Header(header: "Ground Check Settings")]
        [SerializeField]
        private CheckMethod groundCheckMethod = CheckMethod.Raycast;
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float checkDistance = 0.2f;
        [SerializeField] private float overlapRadius = 0.25f;
        [SerializeField] private LayerMask groundLayer;

        [Header(header: "Side Obstacle Check Settings")]
        [SerializeField]
        private float sideObstacleDistance = 0.5f;
        [SerializeField] private LayerMask obstacleLayer;

        [Header(header: "Player Detection Settings")]
        [SerializeField]
        private CheckMethod playerCheckMethod = CheckMethod.Raycast;
        [SerializeField] private float playerDetectDistance = 1f;
        [SerializeField] private LayerMask playerLayer;

        [Header(header: "Debug")]
        [SerializeField]
        private bool drawDebugRay;
        [SerializeField] private bool drawGizmos = true;

        private readonly Collider2D[] playerDetectionBufferLeft = new Collider2D[4];
        private readonly Collider2D[] playerDetectionBufferRight = new Collider2D[4];

        public bool isGrounded { get; private set; }
        public bool isBlockedLeft { get; private set; }
        public bool isBlockedRight { get; private set; }
        public bool hasPlayerLeft { get; private set; }
        public bool hasPlayerRight { get; private set; }

        #region Gizmos Visualization

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!drawGizmos || groundCheckPoint == null) return;

            // GROUND CHECK GIZMO
            Gizmos.color = Color.yellow;
            switch (groundCheckMethod)
            {
                case CheckMethod.Raycast:
                    Gizmos.DrawLine(from: groundCheckPoint.position,
                        to: groundCheckPoint.position + Vector3.down * checkDistance);
                    break;

                case CheckMethod.OverlapCircle:
                    Gizmos.DrawWireSphere(center: groundCheckPoint.position, radius: overlapRadius);
                    break;
            }

            // OBSTACLE CHECKS - RED LINES LEFT/RIGHT
            if (obstacleLayer != LayerMask.NameToLayer(layerName: "Default"))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(from: groundCheckPoint.position,
                    to: groundCheckPoint.position + Vector3.left * sideObstacleDistance);

                Gizmos.DrawLine(from: groundCheckPoint.position,
                    to: groundCheckPoint.position + Vector3.right * sideObstacleDistance);
            }



            // PLAYER CHECKS - BLUE LINES LEFT/RIGHT OR SPHERES IF OVERLAP MODE.
            Gizmos.color = Color.blue;

            Vector3 upOffsetPos = groundCheckPoint.position + Vector3.up * 0.05f;

            if (playerLayer != LayerMask.NameToLayer(layerName: "Default"))
            {
                switch (playerCheckMethod)
                {
                    case CheckMethod.Raycast:
                        Gizmos.DrawLine(from: upOffsetPos,
                            to: upOffsetPos + Vector3.left * playerDetectDistance);

                        Gizmos.DrawLine(from: upOffsetPos,
                            to: upOffsetPos + Vector3.right * playerDetectDistance);
                        break;


                    case CheckMethod.OverlapCircle:
                        Gizmos.DrawWireSphere(center: upOffsetPos +
                            Vector3.left * playerDetectDistance * 0.5f,
                            radius: overlapRadius);

                        Gizmos.DrawWireSphere(center: upOffsetPos +
                            Vector3.right * playerDetectDistance * 0.5f,
                            radius: overlapRadius);
                        break;
                }
            }
        }
#endif

        #endregion

        #region Initialization

        private void InitializeGroundCheckPoint()
        {
            if (groundCheckPoint != null) return;

            Transform found = transform.Find(n: "ObjectChecker");
            if (found != null)
            {
                groundCheckPoint = found;
            }
            else
            {
#if UNITY_EDITOR
                GameObject obj = new GameObject(name: "ObjectChecker");
                Undo.RegisterCreatedObjectUndo(objectToUndo: obj, name: "Create ObjectChecker");
#else
       GameObject obj = new GameObject("ObjectChecker");
#endif
                obj.transform.SetParent(p: transform);
                obj.transform.localPosition = Vector3.zero;
                groundCheckPoint = obj.transform;

#if UNITY_EDITOR
                EditorUtility.SetDirty(target: this);
#endif
            }
        }

        #endregion

        #region Ground Checking

        private void CheckGround()
        {
            switch (groundCheckMethod)
            {
                case CheckMethod.Raycast:
                    RaycastHit2D hit =
                        Physics2D.Raycast(origin: groundCheckPoint.position,
                            direction: Vector2.down,
                            distance: checkDistance,
                            layerMask: groundLayer);

                    isGrounded = hit.collider;

                    if (drawDebugRay)
                    {
                        Color rayColor = isGrounded ? Color.green : Color.red;
                        Debug.DrawRay(start: groundCheckPoint.position,
                            dir: Vector2.down * checkDistance,
                            color: rayColor);
                    }
                    break;

                case CheckMethod.OverlapCircle:
                    Collider2D overlap =
                        Physics2D.OverlapCircle(point: groundCheckPoint.position,
                            radius: overlapRadius,
                            layerMask: groundLayer);

                    isGrounded = overlap;

                    if (drawDebugRay)
                    {
                        Color color = isGrounded ? Color.green : Color.red;
                        Debug.DrawLine(start: groundCheckPoint.position,
                            end: groundCheckPoint.position + Vector3.down * 0.01f,
                            color: color);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Obstacle Checking

        private void CheckObstacles()
        {
            Vector3 origin = groundCheckPoint.position;

            RaycastHit2D leftHit =
                Physics2D.Raycast(origin: origin, direction: Vector2.left, distance: sideObstacleDistance, layerMask: obstacleLayer);

            RaycastHit2D rightHit =
                Physics2D.Raycast(origin: origin, direction: Vector2.right, distance: sideObstacleDistance, layerMask: obstacleLayer);

            isBlockedLeft = leftHit.collider;
            isBlockedRight = rightHit.collider;

#if UNITY_EDITOR
            if (!drawDebugRay) return;

            Debug.DrawRay(start: origin,
                dir: Vector2.left * sideObstacleDistance,
                color: isBlockedLeft ? Color.red : new Color(r: 1f, g: .5f, b: .5f));

            Debug.DrawRay(start: origin,
                dir: Vector2.right * sideObstacleDistance,
                color: isBlockedRight ? Color.red : new Color(r: 1f, g: .5f, b: .5f));
#endif
        }

        #endregion

        #region Player Checking

        private void CheckPlayers()
        {
            Vector3 originL = groundCheckPoint.position + Vector3.up * 0.05f;
            Vector3 originR = originL;

            switch (playerCheckMethod)
            {
                case CheckMethod.Raycast:
                    RaycastHit2D hitL =
                        Physics2D.Raycast(origin: originL, direction: Vector2.left, distance: playerDetectDistance, layerMask: playerLayer);

                    RaycastHit2D hitR =
                        Physics2D.Raycast(origin: originR, direction: Vector2.right, distance: playerDetectDistance, layerMask: playerLayer);

                    hasPlayerLeft = hitL.collider;
                    hasPlayerRight = hitR.collider;

                    if (drawDebugRay)
                    {
                        Debug.DrawRay(start: originL,
                            dir: Vector2.left * playerDetectDistance,
                            color: hasPlayerLeft ? Color.blue : new Color(r: 0.6f, g: 0.6f, b: .9f));

                        Debug.DrawRay(start: originR,
                            dir: Vector2.right * playerDetectDistance,
                            color: hasPlayerRight ? Color.blue : new Color(r: 0.6f, g: .6f, b: .9f));
                    }

                    break;


                case CheckMethod.OverlapCircle:
                    Array.Clear(array: playerDetectionBufferRight, index: 0, length: playerDetectionBufferRight.Length);

                    ContactFilter2D contactFilter = new ContactFilter2D();
                    contactFilter.SetLayerMask(layerMask: playerLayer);

                    int countLeft =
                        Physics2D.OverlapCircle(
                            point: originL + Vector3.left * (playerDetectDistance * 0.5f),
                            radius: overlapRadius,
                            contactFilter: contactFilter,
                            results: playerDetectionBufferLeft);

                    int countRight =
                        Physics2D.OverlapCircle(
                            point: originR + Vector3.right * (playerDetectDistance * 0.5f),
                            radius: overlapRadius,
                            contactFilter: contactFilter,
                            results: playerDetectionBufferRight);

                    hasPlayerLeft = countLeft > 0;
                    hasPlayerRight = countRight > 0;

#if UNITY_EDITOR
                    if (drawDebugRay)
                    {
                        Debug.DrawLine(start: originL,
                            end: originL + Vector3.left * playerDetectDistance,
                            color: hasPlayerLeft ? Color.cyan : new Color(r: .4f, g: .4f, b: .8f));

                        Debug.DrawLine(start: originR,
                            end: originR + Vector3.right * playerDetectDistance,
                            color: hasPlayerRight ? Color.cyan : new Color(r: .4f, g: .4f, b: .8f));
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

        private void Awake()
        {
            // InitializeGroundCheckPoint();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            InitializeGroundCheckPoint();
            EditorUtility.SetDirty(target: this);
        }
#endif

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                InitializeGroundCheckPoint();
            }
        }
#endif

        private void Update()
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
