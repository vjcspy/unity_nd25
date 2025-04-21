using JetBrains.Annotations;
using ND25.Gameplay.Character.Common.MethodInterface;
using ND25.Gameplay.Skills.Base;
using ND25.Util.Common.Enum;
using R3;
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

        public void TryCast(Vector2 direction, [CanBeNull] GameObject target)
        {
            if (!IsReady)
            {
                return;
            }

            Cast(target: target, direction: direction);
            cooldownRemaining = skillData.cooldown;
        }

        private void Cast(Vector2 direction, [CanBeNull] GameObject target)
        {
            skillData.Cast(owner: gameObject, target: target, direction: direction);
        }
    }

    public class SkillManager : MonoBehaviour
    {
        [Header(header: "Skills")]
        [SerializeField] private List<SkillData> availableSkills;

        [Header(header: "Precast: Aim Dots")]
        [SerializeField] private GameObject preCastAimDotPrefab;
        [SerializeField] private int preCastAimDotLength = 3;
        [SerializeField] private float preCastAimDotSpace = 0.02f;


        private readonly Dictionary<SkillId, SkillInstance> skillInstances = new Dictionary<SkillId, SkillInstance>();
        private IFacingDirection _facingDirection;
        private Camera _mainCamera;
        private SkillData _preCaSkillData;
        private GameObject[] _preCastAimDots;
        private DisposableBag disposable;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _facingDirection = gameObject.GetComponent<IFacingDirection>();
            foreach (SkillData skillData in availableSkills)
            {
                SkillInstance skillInstance = new SkillInstance(gameObject: gameObject, skillData: skillData);
                skillInstances.Add(key: skillData.id, value: skillInstance);
            }
        }

        private void Start()
        {
            Observable.Interval(period: TimeSpan.FromMilliseconds(value: 50))
                .Subscribe(onNext: x =>
                {
                    UpdateAimDotPosition();
                })
                .AddTo(bag: ref disposable);
        }

        private void Update()
        {
            foreach (SkillInstance skill in skillInstances.Values)
            {
                skill.Update(deltaTime: Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            disposable.Dispose();
        }

        public void CastSkill(SkillId skillId)
        {
            if (skillInstances.TryGetValue(key: skillId, value: out SkillInstance skillInstance))
            {
                _preCaSkillData = null;
                ToggleAimDots(state: false);

                switch (skillInstance.skillData.preCastSkillType)
                {
                    case PreCastSkillType.Aim:
                        skillInstance.TryCast(target: GetCurrentTarget(), direction: GetAimDirection());
                        break;
                    case PreCastSkillType.None:
                        skillInstance.TryCast(target: GetCurrentTarget(), direction: GetCurrentFacingDirection());
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

        public void PreCastSkill(SkillId skillId)
        {
            if (skillInstances.TryGetValue(key: skillId, value: out SkillInstance skillInstance))
            {
                switch (skillInstance.skillData.preCastSkillType)
                {
                    case PreCastSkillType.None:
                        skillInstance.TryCast(target: GetCurrentTarget(), direction: GetCurrentFacingDirection());
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

        private Vector2 GetCurrentFacingDirection()
        {
            return transform.rotation * new Vector2(x: (int)_facingDirection.GetCurrentFacingDirection(), y: 0);
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
                _preCastAimDots = new GameObject[preCastAimDotLength];
                for (int i = 0; i < preCastAimDotLength; i++)
                {
                    _preCastAimDots[i] = Instantiate(
                        original: preCastAimDotPrefab,
                        position: transform.position,
                        rotation: Quaternion.identity,
                        parent: transform
                    );
                    _preCastAimDots[i].SetActive(value: false);
                }
            }
        }

        public Vector2 GetAimDirection()
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
                + Physics2D.gravity * (_preCaSkillData.gravityScale * (0.5f * (t * t)));

            return position;
        }

        private void UpdateAimDotPosition()
        {
            if (!_preCaSkillData) return;
            for (int i = 0; i < _preCastAimDots.Length; i++)
            {
                GameObject aimDot = _preCastAimDots[i];
                aimDot.transform.position = GetAimDotPosition(t: i * preCastAimDotSpace);
            }
            _facingDirection.SetCurrentFacingDirection(direction: Direction.ConvertToXDirection(velocity: GetAimDirection().x));
        }

        #endregion

    }
}
