using ND25.Gameplay.Skills.Base;
using UnityEngine;
namespace ND25.Gameplay.Skills.Data
{
    [CreateAssetMenu(menuName = "Skills/Fireball")]
    public class Fireball : SkillData
    {
        [Header(header: "Fireball Settings")]
        public float speed = 10f;

        public override void Activate(Transform transform, Vector2 direction)
        {
            throw new System.NotImplementedException();
        }
    }
}
