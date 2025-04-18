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
        [SerializeField] public GameObject preCastPrefab;
        [SerializeField] public int preCastNumberOfObjects = 10;
        [SerializeField] public float preCastSpaceBetweenObjects;

        [Header(header: "Skill Physics")]
        [SerializeField] public Vector2 launchForce;
        [SerializeField] public Vector2 gravity = new Vector2(x: 0, y: -9.81f);
        [SerializeField] public Vector2 maxDistance;
        [SerializeField] public Vector2 aoeSize;

        public abstract void Cast(GameObject owner, [CanBeNull] GameObject target);
    }
}
