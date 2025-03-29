using Core.XLua;
using UnityEngine;
namespace Character.Player
{
    public class WarriorPlayerActor : MonoXState
    {
        private ILuaMonoXState luaMonoXState;
        private Animator animator;

        protected override string ModuleName => "character.xstate.warrior.xstate";

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
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
    }
}
