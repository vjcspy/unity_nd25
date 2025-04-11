using ND25.Core.XMachine;
using System;
namespace ND25.Component.Character.Player.States
{
    public class PlayerJumpState : PlayerAirState
    {

        public PlayerJumpState(Enum id, XMachineActor<PlayerContext> actor) : base(id: id, actor: actor)
        {
        }
        internal override void Entry()
        {
            base.Entry();
            PlayerActor playerActor = (PlayerActor)actor;
            playerActor.ForceJump();
        }
        internal override bool Guard()
        {
            return actor.objectChecker.isGrounded;
        }
    }
}
