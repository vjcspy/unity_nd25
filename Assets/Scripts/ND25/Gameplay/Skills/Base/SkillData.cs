using UnityEngine;
namespace ND25.Gameplay.Skills.Base
{
    public abstract class SkillData : ScriptableObject
    {
        [Header(header: "Skill Info")]
        [SerializeField] public SkillId id;
        [SerializeField] public string skillName;
        [SerializeField] public float cooldown;
        [SerializeField] public GameObject prefab;

        public abstract void Activate(Transform transform, Vector2 direction);

    }
}
