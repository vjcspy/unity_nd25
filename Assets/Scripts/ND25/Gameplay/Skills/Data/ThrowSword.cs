using ND25.Gameplay.Skills.Base;
using UnityEngine;
namespace ND25.Gameplay.Skills.Data
{
    [CreateAssetMenu(menuName = "Skills/ThrowSword")]
    public class ThrowSword : SkillData
    {
        [Header(header: "Throw Sword Settings")]
        [SerializeField] public float swordGravity = 9.81f;
        public override void Activate(Vector2 position, Vector2 direction)
        {
            // Implement the logic to activate the skill
            // This could involve instantiating a prefab, applying effects, etc.
            GameObject skillInstance = Instantiate(original: prefab, position: position, rotation: Quaternion.identity);
            Rigidbody2D rb = skillInstance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction.normalized * 100f; // Example speed
            }
        }
    }
}
