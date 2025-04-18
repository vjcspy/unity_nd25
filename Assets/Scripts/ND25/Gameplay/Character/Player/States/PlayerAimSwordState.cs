using ND25.Core.XMachine;
using ND25.Gameplay.Skills;
using ND25.Gameplay.Skills.Base;
using System;
namespace ND25.Gameplay.Character.Player.States
{
    public class PlayerAimSwordState : XMachineState<PlayerContext>
    {
        private readonly SkillManager skillManager;
        public PlayerAimSwordState(Enum id, XMachineActor<PlayerContext> actor) : base(id: id, actor: actor)
        {
            skillManager = actor.GetComponentInParent<SkillManager>();
        }

        internal override void Entry()
        {
            base.Entry();
            skillManager.PreCastSkill(skillId: SkillId.ThrowSword);
        }
    }
}
