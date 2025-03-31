using Core.XLua;
using UnityEngine;
using XLua;

namespace Character.Actor
{
    [LuaCallCSharp]
    public class PlayerActor : LuaMono
    {
        private Rigidbody2D rb;

        protected override string ModuleName => "character.player";

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
        }

        public void TryJumpIfGrounded()
        {
            if (!rb.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
            var randomForce = Random.Range(0.1f, 1f);
            rb.AddForce(new Vector2(0, randomForce), ForceMode2D.Impulse);
        }
    }
}