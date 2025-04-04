using Core.XLua;
using UnityEngine;
using XLua;

namespace ND25.Character.Actor
{
    [LuaCallCSharp]
    public class WarriorPlayerActor : StateMachineActorMono
    {

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 2f;

        private Animator animator;
        private Rigidbody2D rb;
        private GroundChecker groundChecker;

        private float horizontalInput;

        protected override string ModuleName
        {
            get => "character.xstate.warrior.warrior_state_machine";
        }

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundChecker>();

        }

        [LuaCallable]
        public void UpdateAnimatorParams(LuaTable animatorParams)
        {
            foreach (var key in animatorParams.GetKeys())
            {
                animatorParams.Get(key, out object value);
                if (key is string keyString)
                {
                    if (value is bool boolVal)
                    {
                        animator.SetBool(keyString, boolVal);
                    }
                    else if (value is float floatVal)
                    {
                        animator.SetFloat(keyString, floatVal);
                    }
                    else if (value is double doubleVal)
                    {
                        animator.SetFloat(keyString, (float)doubleVal);
                    }
                    else
                    {
                        Debug.Log("Unsupport type: " + value.GetType());
                    }
                }
            }
        }

        [LuaCallable]
        public void HandleActionMove()
        {
            horizontalInput = Input.GetAxis("Horizontal");

            if (horizontalInput != 0)
            {
                luaStateMachineMono.Dispatch("move");
            }
            else
            {
                luaStateMachineMono.Dispatch("idle");
            }

            Vector2 newVelocity = new(horizontalInput * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = newVelocity;
        }

        [LuaCallable]
        public void HandleActionJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && groundChecker.IsGrounded)
            {
                luaStateMachineMono.Dispatch("jump");
            }
        }
        [LuaCallable]
        public void ForceJump()
        {
            if (groundChecker.IsGrounded)
            {
                groundChecker.ForceCheck(); // Reset ground check
                Vector2 jumpForceVector = Vector2.up * jumpForce;
                rb.AddForce(jumpForceVector, ForceMode2D.Impulse);
            }
        }

        [LuaCallable]
        public void HandleFall()
        {
            // TODO: Should not check by compare y velocity
            if (rb.linearVelocity.y < 0 && groundChecker.IsGrounded)
            {
                luaStateMachineMono.Dispatch("grounded");
            }
        }

        [LuaCallable]
        public void CheckAndUpdateGroundedInfo()
        {
            animator.SetBool("jump", !groundChecker.IsGrounded);
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
        }
    }
}
