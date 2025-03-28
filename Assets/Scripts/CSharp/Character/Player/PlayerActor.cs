using CSharp.Core.XLua;
using JetBrains.Annotations;
using UnityEngine;
using XLua;

namespace CSharp.Character.Player
{
    [LuaCallCSharp]
    public class PlayerActor : LuaMono
    {
        private Rigidbody2D rb;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
        }

        [UsedImplicitly]
        public void TryJumpIfGrounded()
        {
            if (!rb.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
            var randomForce = Random.Range(0.1f, 1f);
            rb.AddForce(new Vector2(0, randomForce), ForceMode2D.Impulse);
        }
    }
}