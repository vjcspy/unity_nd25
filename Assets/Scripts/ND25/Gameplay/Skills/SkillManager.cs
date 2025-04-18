using JetBrains.Annotations;
using ND25.Gameplay.Skills.Base;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace ND25.Gameplay.Skills
{
    public class SkillInstance
    {
        private readonly GameObject gameObject;
        public readonly SkillData skillData;
        private float cooldownRemaining;

        public SkillInstance(GameObject gameObject, SkillData skillData)
        {
            this.gameObject = gameObject;
            this.skillData = skillData;
        }

        private bool IsReady
        {
            get
            {
                return cooldownRemaining <= 0f;
            }
        }

        public void Update(float deltaTime)
        {
            if (cooldownRemaining > 0f)
            {
                cooldownRemaining -= deltaTime;
            }
        }

        public void TryCast([CanBeNull] GameObject target)
        {
            if (!IsReady)
            {
                return;
            }

            Cast(target: target);
            cooldownRemaining = skillData.cooldown;
        }

        private void Cast([CanBeNull] GameObject target)
        {
            skillData.Cast(owner: gameObject, target: target);
        }
    }

    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private List<SkillData> availableSkills;

        private readonly Dictionary<SkillId, SkillInstance> skillInstances = new Dictionary<SkillId, SkillInstance>();
        private Camera _mainCamera;
        private SkillData _preCaSkillData;
        private GameObject[] _preCastAimDots;

        private void Awake()
        {
            _mainCamera = Camera.main;
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

        private void FixedUpdate()
        {
            UpdateAimDotPosition();
        }

        public void CastSkill(SkillId skillId, [CanBeNull] GameObject target)
        {
            if (skillInstances.TryGetValue(key: skillId, value: out SkillInstance skillInstance))
            {
                skillInstance.TryCast(target: target);
                _preCaSkillData = null;
                ToggleAimDots(state: false);
            }
            else
            {
                Debug.LogWarning(message: $"Skill {skillId} not found.");
            }
        }

        public void PreCastSkill(SkillId skillId)
        {
            if (skillInstances.TryGetValue(key: skillId, value: out SkillInstance skillInstance))
            {
                switch (skillInstance.skillData.preCastSkillType)
                {
                    case PreCastSkillType.None:
                        skillInstance.TryCast(target: GetCurrentTarget());
                        break;
                    case PreCastSkillType.Aim:
                        // Implement logic to show aim dots
                        _preCaSkillData = skillInstance.skillData;
                        InitAimDots();
                        ToggleAimDots(state: true);
                        break;
                    case PreCastSkillType.Target:
                        break;
                    case PreCastSkillType.Direction:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Debug.LogWarning(message: $"Skill {skillId} not found.");
            }
        }

        private GameObject GetCurrentTarget()
        {
            // Implement logic to get the current target
            return null;
        }

        #region Precast: Aim Dots

        private void InitAimDots()
        {
            if (_preCastAimDots == null)
            {
                _preCastAimDots = new GameObject[_preCaSkillData.preCastNumberOfObjects];
                for (int i = 0; i < _preCaSkillData.preCastNumberOfObjects; i++)
                {
                    _preCastAimDots[i] = Instantiate(
                        original: _preCaSkillData.preCastPrefab,
                        position: transform.position,
                        rotation: Quaternion.identity,
                        parent: transform
                    );
                    _preCastAimDots[i].SetActive(value: false);
                }
            }
        }

        private Vector2 GetAimDirection()
        {
            Vector2 ownerPos = transform.position;
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(position: screenPosition);

            return mouseWorldPosition - ownerPos;
        }

        private void ToggleAimDots(bool state)
        {
            foreach (GameObject aimDot in _preCastAimDots)
            {
                aimDot.SetActive(value: state);
            }
        }

        private Vector2 GetAimDotPosition(float t)
        {
            Vector2 position = (Vector2)transform.position
                + new Vector2(
                    x: GetAimDirection().normalized.x * _preCaSkillData.launchForce.x,
                    y: GetAimDirection().normalized.y * _preCaSkillData.launchForce.y
                ) * t
                + _preCaSkillData.gravity * (0.5f * (t * t));

            return position;
        }

        private void UpdateAimDotPosition()
        {
            if (!_preCaSkillData) return;
            for (int i = 0; i < _preCastAimDots.Length; i++)
            {
                GameObject aimDot = _preCastAimDots[i];
                aimDot.transform.position = GetAimDotPosition(t: i * _preCaSkillData.preCastSpaceBetweenObjects);
            }
        }

        #endregion

    }
}
