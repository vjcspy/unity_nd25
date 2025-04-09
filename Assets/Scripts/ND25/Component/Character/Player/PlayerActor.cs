using ND25.Component.Character.Player.Effects;
using ND25.Component.Character.Player.States;
using ND25.Component.Character.Warrior;
using ND25.Core.XMachine;
using System;
using UnityEngine;
namespace ND25.Component.Character.Player
{
    public class PlayerActor : XMachineActor<PlayerContext>
    {

        private Animator animator;
        internal WarriorAnimatorParam animatorParam;
        internal ObjectChecker objectChecker;
        internal PCControls pcControls;
        internal Rigidbody2D rb;

        #region Configuration

        private void OnEnable()
        {
            pcControls.GamePlay.Enable();
        }
        private void OnDisable()
        {
            pcControls.GamePlay.Disable();
        }

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            objectChecker = GetComponent<ObjectChecker>();
            pcControls = new PCControls();
            animatorParam = new WarriorAnimatorParam(animator: animator);
        }
        protected override PlayerContext ConfigureInitialContext()
        {
            return new PlayerContext();
        }

        protected override XMachineState<PlayerContext>[] ConfigureMachineStates()
        {
            return new XMachineState<PlayerContext>[]
            {
                new PlayerIdleState(id: PlayerState.Idle, actor: this),
                new PlayerMoveState(id: PlayerState.Move, actor: this),
                new PlayerPrimaryAttackState(id: PlayerState.PrimaryAttack, actor: this)
            };
        }
        protected override XMachineEffect<PlayerContext>[] ConfigureMachineEffects()
        {
            return new XMachineEffect<PlayerContext>[]
            {
                new PlayerMoveEffect(actor: this)
            };
        }

        protected override Enum ConfigureInitialStateId()
        {
            return PlayerState.Idle;
        }

        #endregion

        #region Inspector

        [Header(header: "Movement Settings")]
        [SerializeField]
        internal float moveSpeed = 5f;
        [SerializeField]
        internal float jumpForce = 2f;

        #endregion

    }
}
