using Cysharp.Threading.Tasks;
using ND25.Core.ReactiveMachine;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace ND25.Character.Actor
{
    internal enum WarriorAction
    {
        HandleXInput,
        UpdateAnimatorParams,
        ForceJump,

        WhenXInputChange, // When naming convention, use "When" prefix for dispatching events
        WhenFallGround,   // When naming convention, use "When" prefix for dispatching events
        WhenYInputChange  // When naming convention, use "When" prefix for dispatching events
    }

    // internal enum WarriorState
    // {
    //     idle,
    //     move,
    //     jump,
    // }

    internal enum WarriorEvent
    {
        move,
        idle,
        jump
    }

    public static class WarriorAnimatorParams
    {
        public static readonly int idle = Animator.StringToHash("idle");
        public static readonly int move = Animator.StringToHash("move");
        public static readonly int jump = Animator.StringToHash("jump");
        public static readonly int yVelocity = Animator.StringToHash("yVelocity");
    }


    public class WarriorContext
    {
        public float yVelocity;

        public float lastJumpTime;


        public WarriorContext(float lastJumpTime = 0f, float yVelocity = 0f)
        {
            this.yVelocity = yVelocity;
            this.lastJumpTime = lastJumpTime;
        }
    }


    /*
     * This version is using ReactiveMachine
     */
    public class WarriorActor : ReactiveMachineMono<WarriorContext>
    {

        [Header("Movement Settings")]
        [SerializeField]
        float moveSpeed = 5f;
        [SerializeField] float jumpForce = 2f;

        Animator animator;
        GroundChecker groundChecker;
        Rigidbody2D rb;

        float xInput;
        protected override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundChecker>();

            HandleContextChange();
        }
        protected override string GetJsonFileName()
        {
            return "warrior_config";
        }
        protected override object[] GetActionHandlers()
        {
            return new object[]
            {
                this
            };
        }
        protected override WarriorContext GetInitContext()
        {
            return new WarriorContext();
        }

        void HandleContextChange()
        {
            machine
                .context
                .ThrottleLast(TimeSpan.FromMilliseconds(200))
                .Subscribe(
                    context =>
                    {
                        UniTask.Post(() =>
                        {
                            animator.SetFloat(WarriorAnimatorParams.yVelocity, context.yVelocity);
                        });
                    },
                    error =>
                    {
                        Debug.LogError($"[WarriorActor] HandleContextChange Error: {error}");
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler HandleXInput()
        {
            return upstream => upstream
                .OfAction(WarriorAction.HandleXInput)
                .Select(
                    _ =>
                    {
                        xInput = Input.GetAxis("Horizontal");
                        float yVelocity = rb.linearVelocity.y;
                        Vector2 newVelocity = new Vector2(xInput * moveSpeed, yVelocity);
                        rb.linearVelocity = newVelocity;

                        machine.SetContext(
                            context =>
                            {
                                context.yVelocity = yVelocity;

                                return context;
                            }
                        );

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler UpdateAnimatorParams()
        {
            var cachedKey = new Dictionary<string, int>();

            return upstream => upstream
                .OfAction(WarriorAction.UpdateAnimatorParams)
                .Select(
                    action =>
                    {

                        if (action.payload == null)
                        {
                            return ReactiveMachineCoreAction.Empty;
                        }

                        foreach ((string keyString, object value) in action.payload)
                        {
                            if (!cachedKey.ContainsKey(keyString))
                            {
                                cachedKey[keyString] = Animator.StringToHash(keyString);
                            }
                            // Debug.Log("Update animator key: " + keyString);
                            switch (value)
                            {
                                case bool boolVal:
                                    animator.SetBool(cachedKey[keyString], boolVal);
                                    break;
                                case float floatVal:
                                    animator.SetFloat(cachedKey[keyString], floatVal);
                                    break;
                                case double doubleVal:
                                    animator.SetFloat(cachedKey[keyString], (float)doubleVal);
                                    break;
                                default:
                                    Debug.Log("Unsupported type: " + value.GetType());
                                    break;
                            }
                        }

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler WhenXInputChange()
        {
            return upstream => upstream
                .OfAction(WarriorAction.WhenXInputChange)
                .Select(
                    _ =>
                    {
                        machine.DispatchEvent(xInput != 0 ? WarriorEvent.move : WarriorEvent.idle);
                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler WhenFallGround()
        {
            return upstream => upstream
                .OfAction(WarriorAction.WhenFallGround)
                .Where(action => machine.context.Value.lastJumpTime < Time.time - 0.2f)
                .ThrottleLast(TimeSpan.FromMilliseconds(200))
                .Select(
                    _ =>
                    {
                        Debug.Log($"here {groundChecker.isGrounded}");

                        if (groundChecker.isGrounded)
                        {
                            UniTask.Post(() =>
                            {
                                machine.DispatchEvent(WarriorEvent.idle);
                            });
                        }

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler ForceJump()
        {
            return upstream => upstream
                .OfAction(WarriorAction.ForceJump)
                .Select(
                    _ =>
                    {
                        if (!groundChecker.isGrounded)
                        {
                            return ReactiveMachineCoreAction.Empty;
                        }
                        Vector2 jumpForceVector = Vector2.up * jumpForce;
                        rb.AddForce(jumpForceVector, ForceMode2D.Impulse);

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler WhenYInputChange()
        {
            return upstream => upstream
                .OfAction(WarriorAction.WhenYInputChange)
                .Select(
                    _ =>
                    {
                        if (!Input.GetKeyDown(KeyCode.Space) || !groundChecker.isGrounded)
                        {
                            return ReactiveMachineCoreAction.Empty;
                        }

                        machine.DispatchEvent(WarriorEvent.jump);
                        machine.SetContext(
                            context =>
                            {
                                context.lastJumpTime = Time.time;

                                return context;
                            }
                        );

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }
    }
}
