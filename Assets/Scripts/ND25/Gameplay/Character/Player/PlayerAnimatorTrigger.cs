using ND25.Gameplay.Skills;
using UnityEngine;
namespace ND25.Gameplay.Character.Player
{
    public class PlayerAnimatorTrigger: MonoBehaviour
    {
        private PlayerActor playerActor;
        private SkillManager skillManager;

        private void Awake()
        {
            playerActor = GetComponentInParent<PlayerActor>();
            skillManager = GetComponentInParent<SkillManager>();
        }

        public void OnAnimationFinish()
        {
            playerActor.machine.GetCurrentState().OnAnimationFinish();
        }

        public void ThrowSword()
        {
            skillManager.ActivateSkill(SkillId.ThrowSword);
        }
    }
}
