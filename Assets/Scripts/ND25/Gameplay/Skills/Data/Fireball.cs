using ND25.Gameplay.Skills.Base;
using UnityEngine;
namespace ND25.Gameplay.Skills.Data
{
    [CreateAssetMenu(menuName = "Skills/Fireball")]
    public class Fireball : SkillData
    {
        [Header(header: "Fireball Settings")]
        public GameObject fireballPrefab;
        public float speed = 10f;

        protected override void Activate()
        {
            GameObject fireball = Instantiate(original: fireballPrefab, position: owner.transform.position + Vector3.forward * 1.5f, rotation: Quaternion.identity);
            // Gắn thêm logic bay / damage ở prefab fireball.
            Debug.Log(message: "Fireball casted!");
        }
    }
}
