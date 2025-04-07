using ND25.Core.ReactiveMachine;
using UnityEngine;
namespace ND25.Component.Character.Common
{
    public abstract class Entity<T> : ReactiveMachineComponent<T>
    {
        ObjectChecker objectChecker;

        Rigidbody2D rb;
        public float curDirection { get; private set; } = 1;
        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
        }

        public void Flip()
        {
            switch (rb.linearVelocity.x)
            {
                case > 0:
                    transform.localScale = new Vector3(1, 1, 1);
                    curDirection = 1;
                    break;
                case < 0:
                    transform.localScale = new Vector3(-1, 1, 1);
                    curDirection = -1;
                    break;
                default:
                    transform.localScale = transform.localScale;
                    break;
            }
        }

        public ObjectChecker GetGroundChecker()
        {
            if (!objectChecker)
            {
                objectChecker = gameObject.AddComponent<ObjectChecker>();
            }

            return objectChecker;
        }
    }
}
