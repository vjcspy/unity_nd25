using ND25.Component.Character.Player.Effects;
using ND25.Component.Character.Player.States;
using ND25.Core.XMachine;
using ND25.Util.Common.Enum;
using R3;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace ND25.Component.Character.Player
{
    public class PlayerActor : XMachineActor<PlayerContext>
    {

        private Animator animator;
        private PlayerAnimatorParam animatorParam;
        internal ObjectChecker objectChecker;
        internal PCControls pcControls;
        internal Rigidbody2D rb;

        private void HandleAnimation()
        {
            machine.reactiveContext.CombineLatest(source2: machine.reactiveCurrentStateId, resultSelector: (context, stateId) => (Context: context, StateId: stateId))
                .ThrottleLast(timeSpan: TimeSpan.FromMilliseconds(value: 100))
                .Subscribe(
                    onNext: x =>
                    {
                        Flip(xVelocity: x.Context.xVelocity);

                        switch (x.StateId)
                        {
                            case PlayerState.Idle:
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.state, value: (int)PlayerAnimatorState.Idle);
                                break;
                            case PlayerState.Move:
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.state, value: (int)PlayerAnimatorState.Move);
                                break;
                            case PlayerState.Air:
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.state, value: (int)PlayerAnimatorState.Air);
                                animatorParam.UpdateFloatParam(param: PlayerAnimatorParamType.yVelocity, value: x.Context.yVelocity);
                                break;
                        }
                    }
                );
        }

        #region ActorCapability

        public void SetVelocity(Vector2 moveInput)
        {
            Vector2 velocity = new Vector2(x: moveInput.x * moveSpeed, y: rb.linearVelocity.y);
            rb.linearVelocity = velocity;
        }
        public void ForceJump()
        {
            if (!objectChecker.isGrounded)
            {
                return;
            }

            Vector2 jumpForceVector = Vector2.up * jumpForce;
            rb.AddForce(force: jumpForceVector, mode: ForceMode2D.Impulse);
        }

        public void Flip(float xVelocity)
        {
            if (Mathf.Approximately(a: xVelocity, b: 0f)) return;

            transform.localScale = xVelocity switch
            {
                > 0 => FacingDirection.FacingRight,
                < 0 => FacingDirection.FacingLeft,
                _ => transform.localScale
            };
        }

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        private static bool ApproximatelyEqual(float a, float b)
        {
            return Mathf.Abs(f: a - b) < 0.001f;
        }

        #endregion


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
            animatorParam = new PlayerAnimatorParam(animator: animator);
        }

        protected override void Start()
        {
            base.Start();
            HandleAnimation();
            LogStateChange();
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
                new PlayerAirState(id: PlayerState.Air, actor: this),
                new PlayerPrimaryAttackState(id: PlayerState.PrimaryAttack, actor: this)
            };
        }
        protected override XMachineEffect<PlayerContext>[] ConfigureMachineEffects()
        {
            return new XMachineEffect<PlayerContext>[]
            {
                new PlayerEffect(actor: this),
                new PlayerMoveEffect(actor: this),
                new PlayerAirEffect(actor: this)
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
        private float moveSpeed = 5f;
        [SerializeField]
        private float jumpForce = 5f;

        #endregion

    }
}
