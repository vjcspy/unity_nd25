using JetBrains.Annotations;
using UnityEngine;
namespace ND25.Gameplay.Skills.Base
{

    public enum PreCastSkillType
    {
        None,
        Aim,
        Target,
        Direction
    }

    public abstract class SkillData : ScriptableObject
    {
        [Header(header: "Skill Info")]
        [SerializeField] public SkillId id;
        [SerializeField] public string skillName;
        [SerializeField] public float cooldown;
        [SerializeField] public GameObject prefab;

        [Header(header: "Skill Preparation")]
        [SerializeField] public PreCastSkillType preCastSkillType;

        [Header(header: "Skill Physics")]
        [SerializeField] public Vector2 launchForce;
        [SerializeField] public float gravityScale = 1;
        [SerializeField] public Vector2 maxDistance;
        [SerializeField] public Vector2 aoeSize;

        public abstract void Cast(GameObject owner, Vector2 direction, [CanBeNull] GameObject target);
    }
}
