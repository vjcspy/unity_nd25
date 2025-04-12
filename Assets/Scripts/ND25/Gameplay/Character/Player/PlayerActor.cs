using ND25.Core.XMachine;
using ND25.Gameplay.Character.Player.Effects;
using ND25.Gameplay.Character.Player.States;
using ND25.Util.Common.Enum;
using R3;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
namespace ND25.Gameplay.Character.Player
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
                        switch (x.StateId)
                        {
                            case PlayerState.Idle:
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.state, value: (int)PlayerAnimatorState.Idle);
                                break;
                            case PlayerState.Move:
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.state, value: (int)PlayerAnimatorState.Move);
                                break;
                            case PlayerState.Air:
                            case PlayerState.Jump:
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.state, value: (int)PlayerAnimatorState.Air);
                                animatorParam.UpdateFloatParam(param: PlayerAnimatorParamType.yVelocity, value: (float)x.Context.yVelocity);
                                break;
                            case PlayerState.PrimaryAttack:
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.state, value: (int)PlayerAnimatorState.PrimaryAttack);
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.primaryAttackCount, value: x.Context.primaryAttackCount);
                                break;
                            case PlayerState.AimSword:
                                animatorParam.UpdateIntParam(param: PlayerAnimatorParamType.state, value: (int)PlayerAnimatorState.AimSword);
                                break;
                        }

                        // Because flip depend on xInput and it's in context
                        Flip();
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
            Vector2 jumpForceVector = Vector2.up * jumpForce;
            rb.AddForce(force: jumpForceVector, mode: ForceMode2D.Impulse);
        }

        private void Flip()
        {
            if (machine.GetContextValue().xInput == XDirection.None)
            {
                return;
            }

            transform.localScale = machine.GetContextValue().xInput switch
            {
                XDirection.Right => FacingDirection.FacingRight,
                XDirection.Left => FacingDirection.FacingLeft,
                _ => transform.localScale
            };
        }

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]

        #endregion

        #region InputListener

        private void OnEnable()
        {
            pcControls.GamePlay.Enable();
            pcControls.GamePlay.PrimaryAttack.started += PrimaryAttackInputListener;
            pcControls.GamePlay.Jump.started += JumpInputListener;
            pcControls.GamePlay.ThrowSword.performed += ThrowSwordInputListener;
            pcControls.GamePlay.ThrowSword.canceled += ThrowSwordInputReleaseListener;
        }
        private void OnDisable()
        {
            pcControls.GamePlay.Disable();
            pcControls.GamePlay.PrimaryAttack.started -= PrimaryAttackInputListener;
            pcControls.GamePlay.Jump.started -= JumpInputListener;
            pcControls.GamePlay.ThrowSword.performed -= ThrowSwordInputListener;
            pcControls.GamePlay.ThrowSword.canceled -= ThrowSwordInputReleaseListener;
        }
        private void PrimaryAttackInputListener(InputAction.CallbackContext context)
        {
            machine.Transition(toStateId: PlayerState.PrimaryAttack);
        }

        private void JumpInputListener(InputAction.CallbackContext context)
        {
            machine.Transition(toStateId: PlayerState.Jump);
        }

        private void ThrowSwordInputListener(InputAction.CallbackContext context)
        {
            machine.Transition(toStateId: PlayerState.AimSword);
        }

        private void ThrowSwordInputReleaseListener(InputAction.CallbackContext context)
        {
            machine.Transition(toStateId: PlayerState.Idle);
        }

        #endregion

        #region Configuration

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
            // LogStateChange();
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
                new PlayerPrimaryAttackState(id: PlayerState.PrimaryAttack, actor: this),
                new PlayerJumpState(id: PlayerState.Jump, actor: this),
                new PlayerAimSwordState(id: PlayerState.AimSword, actor: this),
                new PlayerCatchSwordState(id: PlayerState.CatchSword, actor: this)
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
