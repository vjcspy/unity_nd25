using ND25.Gameplay.Skills.Base;
using UnityEngine;
namespace ND25.Gameplay.Skills.Data
{
    [CreateAssetMenu(menuName = "Skills/ThrowSword")]
    public class ThrowSword : SkillData
    {
        [Header(header: "Throw Sword Settings")]
        [SerializeField] private float mass = 0.01f;

        public override void Cast(GameObject owner, Vector2 direction, GameObject target)
        {
            GameObject skillGO = Instantiate(original: prefab, position: owner.transform.position, rotation: Quaternion.identity);
            Rigidbody2D rb = skillGO.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = gravityScale;
                rb.linearVelocity = direction.normalized * launchForce;
                rb.mass = mass;
            }
        }
    }
}
