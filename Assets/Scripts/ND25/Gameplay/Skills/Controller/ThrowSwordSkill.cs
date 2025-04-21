using UnityEngine;
namespace ND25.Gameplay.Skills.Controller
{
    public class ThrowSwordSkill : MonoBehaviour
    {
        private BoxCollider2D _collider;

        private Rigidbody2D _rb;
        private bool _stuck;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void LateUpdate()
        {
            if (!_stuck)
            {
                transform.right = _rb.linearVelocity;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_stuck)
                _collider.enabled = false;

            // 1. Ngừng vật lý
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // 2. Găm vào đối tượng bị đâm
            // Giữ nguyên world position/rotation khi gắn parent
            transform.SetParent(parent: collision.transform, worldPositionStays: true);

            // 3. Ghi nhớ trạng thái
            _stuck = true;
        }
    }
}
