using UnityEngine;
namespace ND25.Gameplay.Skills.Base
{
    public abstract class SkillData : ScriptableObject
    {
        [Header("Skill Info")]
        public SkillId id;
        public string skillName;
        public float cooldown;
        protected float lastUsedTime = -999f;

        protected GameObject owner;

        protected abstract void Activate();
    }
}
