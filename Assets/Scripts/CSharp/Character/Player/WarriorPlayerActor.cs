using Core.XLua;
using UnityEngine;
using XLua;
namespace Character.Player
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
        public void UpdateAnimator(string stateName, bool value)
        {
            if (animator == null)
            {
                Debug.LogWarning("Animator is null in WarriorPlayerActor");
                return;
            }
            animator.SetBool(stateName, value);
        }

        [LuaCallable]
        public void HandleMove()
        {
            float moveInput = Input.GetAxisRaw("Horizontal");

            if (moveInput != 0)
            {
                luaStateMachineMono.Dispatch("move", null);
            }else{
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
