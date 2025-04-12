using ND25.Gameplay.Skills.Base;
using System.Collections.Generic;
using UnityEngine;
namespace ND25.Gameplay.Skills
{
    public enum SkillId
    {
        Fireball,
        SuperFireball,
        IceSpike,
        LightningBolt
        // Thêm các kỹ năng khác ở đây
    }

    public class Skill
    {
        private readonly GameObject gameObject;
        private readonly SkillData skillData;
        private float cooldownRemaining;

        public Skill(GameObject gameObject, SkillData skillData)
        {
            this.gameObject = gameObject;
            this.skillData = skillData;
        }

        public bool IsReady
        {
            get
            {
                return cooldownRemaining <= 0f;
            }
        }

        public void Update(float deltaTime)
        {
            if (cooldownRemaining > 0f)
                cooldownRemaining -= deltaTime;
            if (cooldownRemaining < 0f)
                cooldownRemaining = 0f;
        }

        public void TryActivate()
        {
            if (!IsReady)
            {
                return;
            }

            Activate();
            cooldownRemaining = skillData.cooldown;
        }

        private void Activate()
        {

            // Nếu dùng Lua:
            // LuaManager.Instance.Call(luaCallback, owner);

            // Trigger animation/sound/fx ở đây nếu cần
        }
    }

    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private List<SkillData> equippedSkills;

        private Dictionary<SkillId, Skill> skills = new Dictionary<SkillId, Skill>();

        private void Awake()
        {
            foreach (SkillData skillData in equippedSkills)
            {
                Skill skill = new Skill(gameObject: gameObject, skillData: skillData);
                skills.Add(skillData.id, skill);
            }
        }
    }
}
