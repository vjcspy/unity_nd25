using JetBrains.Annotations;
using ND25.Gameplay.Character.Common.MethodInterface;
using ND25.Gameplay.Skills.Base;
using UnityEngine;
namespace ND25.Gameplay.Skills.Data
{
    [CreateAssetMenu(menuName = "Skills/ThrowSword")]
    public class ThrowSword : SkillData
    {
        private GameObject[] aimDots;

        public override void Cast(GameObject owner, [CanBeNull] GameObject target)
        {
            IFacingDirection facingDirection = owner.GetComponent<IFacingDirection>();
            if (facingDirection == null) return;

            Vector2 direction = owner.transform.rotation * new Vector2(x: (int)facingDirection.GetCurrentFacingDirection(), y: 0);
            GameObject skillGO = Instantiate(original: prefab, position: owner.transform.position, rotation: Quaternion.identity);
            Rigidbody2D rb = skillGO.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = gravityScale;
                rb.linearVelocity = direction.normalized * launchForce;
            }
        }
    }
}
