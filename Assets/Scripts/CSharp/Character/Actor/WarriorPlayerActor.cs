using Core.XLua;
using UnityEngine;
using XLua;
namespace Character.Actor
{
    [LuaCallCSharp]
    public class WarriorPlayerActor : StateMachineActorMono
    {
        private Animator animator;
        private Rigidbody2D rb;

        protected override string ModuleName => "character.xstate.warrior.warrior_state_machine";

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        protected override void Start()
        {
            base.Start();
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
        public void HandleUserInput()
        {
            float moveInput = Input.GetAxisRaw("Horizontal");

            if (moveInput != 0)
            {
                luaStateMachineMono.Dispatch("move", null);
            }
            else
            {
                luaStateMachineMono.Dispatch("idle", null);
            }

            Vector2 newVelocity = new(moveInput * 5f, rb.linearVelocity.y);
            rb.linearVelocity = newVelocity;
        }

        [LuaCallable]
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
