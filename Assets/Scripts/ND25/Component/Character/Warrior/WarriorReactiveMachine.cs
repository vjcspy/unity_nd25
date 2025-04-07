﻿using ND25.Component.Character.Common;
using ND25.Component.Character.Warrior.Actor;
using ND25.Core.ReactiveMachine;
using UnityEngine;
namespace ND25.Component.Character.Warrior
{
    public class WarriorReactiveMachine : ReactiveMachineWrapper<WarriorContext>
    {

        #region Property

        Animator animator;
        internal ObjectChecker ObjectChecker;
        internal Rigidbody2D rb;
        internal WarriorAnimatorParam animatorParam;

        #endregion

        #region ReactiveMachineWrapper

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
            ObjectChecker = GetComponent<ObjectChecker>();
            animatorParam = new WarriorAnimatorParam(animator);
        }

        protected override void RegisterCustomerHandler()
        {
            machine.RegisterAction(CommonActor.UpdateAnimatorParams(animatorParam));
        }

        protected override string GetJsonFileName()
        {
            return "warrior";
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
