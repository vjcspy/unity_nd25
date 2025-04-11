using ND25.Component.Character.Player.Effects;
using ND25.Component.Character.Player.States;
using ND25.Core.XMachine;
using ND25.Util.Common.Enum;
using R3;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
namespace ND25.Component.Character.Player
{
    public class PlayerActor : XMachineActor<PlayerContext>
    {
        private PlayerAnimatorParam animatorParam;
        private void HandleAnimation()
        {
            machine.reactiveContext.CombineLatest(source2: machine.reactiveCurrentStateId, resultSelector: (context, stateId) => (Context: context, StateId: stateId))
                .Subscribe(
                    onNext: x =>
                    {
                        Flip(xVelocity: x.Context.xVelocity);
                        animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.primaryAttackCount, value: x.Context.primaryAttackCount);

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
                                animatorParam.UpdateFloatParam(param: PlayerAnimatorParamType.yVelocity, value: (float)x.Context.yVelocity);
                                break;
                            case PlayerState.PrimaryAttack:
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.state, value: (int)PlayerAnimatorState.PrimaryAttack);
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

        public void Flip(XDirection xVelocity)
        {
            if (xVelocity == XDirection.None)
            {
                return;
            }

            transform.localScale = xVelocity switch
            {
                XDirection.Right => FacingDirection.FacingRight,
                XDirection.Left => FacingDirection.FacingLeft,
                _ => transform.localScale
            };
        }

        private void PrimaryAttackInputListen(InputAction.CallbackContext context)
        {
            machine.Transition(toStateId: PlayerState.PrimaryAttack);
        }

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]

        #endregion

        #region Configuration

        private void OnEnable()
        {
            pcControls.GamePlay.Enable();
            pcControls.GamePlay.PrimaryAttack.performed += PrimaryAttackInputListen;
        }
        private void OnDisable()
        {
            pcControls.GamePlay.Disable();
            pcControls.GamePlay.PrimaryAttack.performed -= PrimaryAttackInputListen;
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
                new PlayerAirEffect(actor: this),
                new PlayerPrimaryAttackEffect(actor: this)
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
