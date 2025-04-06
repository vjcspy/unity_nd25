using ND25.Character.Warrior.Actor;
using ND25.Core.ReactiveMachine;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
namespace ND25.Character.Warrior
{
    public class WarriorReactiveMachine : ReactiveMachineMono<WarriorContext>
    {

        #region Property

        Animator animator;
        internal GroundChecker groundChecker;
        internal Rigidbody2D rb;
        internal WarriorAnimator warriorAnimator;

        #endregion

        #region ReactiveMachineMono

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundChecker>();
            warriorAnimator = new WarriorAnimator(animator);

            // machine.currentStateName.Subscribe((_name) => { currentState = _name; }, _ => { });
        }
        protected override string GetJsonFileName()
        {
            return "warrior_config";
        }
        protected override object[] GetActionHandlers()
        {
            return new object[]
            {
                new WarriorCommonActor(this), new WarriorAirActor(this), new WarriorPrimaryAttackActor(this)
            };
        }
        protected override WarriorContext GetInitContext()
        {
            return new WarriorContext();
        }

        #endregion

        #region Inspector

        [Header("Movement Settings")]
        [SerializeField]
        public float moveSpeed = 5f;
        [SerializeField]
        public float jumpForce = 2f;

        #endregion

    }
}
