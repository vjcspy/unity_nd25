using ND25.Gameplay.Character.Common;
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
        LightningBolt,
        ThrowSword
        // Thêm các kỹ năng khác ở đây
    }

    public class SkillInstance
    {
        private readonly GameObject gameObject;
        private readonly SkillData skillData;
        private float cooldownRemaining;


        public SkillInstance(GameObject gameObject, SkillData skillData)
        {
            this.gameObject = gameObject;
            this.skillData = skillData;
            entityXDirection = gameObject.GetComponent<EntityXDirection>();
        }
        public EntityXDirection entityXDirection { get; set; }

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

        public void Activate()
        {
            Vector2 position = gameObject.transform.position;
            Vector2 direction = gameObject.transform.rotation * new Vector2(x: (int)entityXDirection.GetCurrentFacingDirection(), y: 0); // Hoặc hướng mà bạn muốn kỹ năng bay
            skillData.Activate(position: position, direction: direction);
        }
    }

    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private List<SkillData> availableSkills;

        private readonly Dictionary<SkillId, SkillInstance> skillInstances = new Dictionary<SkillId, SkillInstance>();

        private void Awake()
        {
            foreach (SkillData skillData in availableSkills)
            {
                SkillInstance skillInstance = new SkillInstance(gameObject: gameObject, skillData: skillData);
                skillInstances.Add(key: skillData.id, value: skillInstance);
            }
        }

        private void Update()
        {
            foreach (SkillInstance skill in skillInstances.Values)
            {
                skill.Update(deltaTime: Time.deltaTime);
            }
        }

        public void ActivateSkill(SkillId skillId)
        {
            if (skillInstances.TryGetValue(key: skillId, value: out SkillInstance skillInstance))
            {
                skillInstance.Activate();
            }
            else
            {
                Debug.LogWarning(message: $"Skill {skillId} not found.");
            }
        }
    }
}
