﻿using UnityEngine;
namespace ND25.Component.Character.Player
{
    public class PlayerAnimatorTrigger: MonoBehaviour
    {
        private PlayerActor playerActor;

        private void Awake()
        {
            playerActor = GetComponentInParent<PlayerActor>();
        }

        public void OnAnimationFinish()
        {
            playerActor.machine.GetCurrentState().OnAnimationFinish();
        }
    }
}
