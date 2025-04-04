using ND25.Core.ReactiveMachine;
using R3;
using System;
using UnityEngine;
namespace ND25.Character.Actor
{
    internal enum WarriorAction
    {
        HandleMoveInput,
        UpdateAnimatorParams,
    }

    internal enum WarriorState
    {
        idle,
        move,
        jump,
    }

    internal enum WarriorEvent
    {
        move,
        idle,
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

        public bool isGrounded;
        public float yVelocity;


        public WarriorContext(bool isGrounded, float yVelocity = 0f)
        {
            this.isGrounded = isGrounded;
            this.yVelocity = yVelocity;
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
                this,
            };
        }
        protected override WarriorContext GetInitContext()
        {
            return new WarriorContext(true);
        }

        void HandleContextChange()
        {
            machine.context
                .Subscribe(
                    context =>
                    {
                        animator.SetFloat(WarriorAnimatorParams.yVelocity, context.yVelocity);
                    },
                    error =>
                    {
                        Debug.LogError($"[WarriorActor] HandleContextChange Error: {error}");
                    }
                );
        }

        void ForceJump()
        {
            if (!groundChecker.isGrounded)
            {
                return;
            }
            Vector2 jumpForceVector = Vector2.up * jumpForce;
            rb.AddForce(jumpForceVector, ForceMode2D.Impulse);
        }

        void HandleJumpInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) && groundChecker.isGrounded)
            {
                ForceJump();
            }
        }

        void HandleXInput()
        {
            xInput = Input.GetAxis("Horizontal");
            Vector2 newVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = newVelocity;

        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler HandleMoveInput()
        {
            return upstream => upstream
                .OfAction(WarriorAction.HandleMoveInput)
                .Select(
                    _ =>
                    {
                        HandleXInput();
                        HandleJumpInput();

                        machine.SetContext(
                            context =>
                            {
                                context.isGrounded = groundChecker.isGrounded;
                                context.yVelocity = rb.linearVelocity.y;

                                return context;
                            }
                        );

                        machine.DispatchEvent(xInput != 0 ? WarriorEvent.move : WarriorEvent.idle);
                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }

        [ReactiveMachineEffect]
        public ReactiveMachineActionHandler UpdateAnimatorParams()
        {
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
                            switch (value)
                            {
                                case bool boolVal:
                                    animator.SetBool(keyString, boolVal);
                                    break;
                                case float floatVal:
                                    animator.SetFloat(keyString, floatVal);
                                    break;
                                case double doubleVal:
                                    animator.SetFloat(keyString, (float)doubleVal);
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
    }
}
