using Core.XLua;
using UnityEngine;
using XLua;
namespace Character.Player
{
    [LuaCallCSharp]
    public class WarriorPlayerActor : XStateMonoBehavior
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

        [LuaCallable]
        public void UpdateAnimator(string stateName, bool value)
        {
            animator.SetBool(stateName, value);
        }

        [LuaCallable]
        public void UpdateAnimator(string stateName, float value)
        {
            animator.SetFloat(stateName, value);
        }

        [LuaCallable]
        public void HandleMove()
        {
            var moveDirection = new Vector2(Input.GetAxis("Horizontal"), rb.linearVelocity.y);
            rb.linearVelocity = moveDirection * 5;
        }

        [LuaCallable]
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
