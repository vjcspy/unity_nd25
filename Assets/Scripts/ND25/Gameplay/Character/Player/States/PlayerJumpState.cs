using ND25.Core.XMachine;
using System;
namespace ND25.Gameplay.Character.Player.States
{
    public class PlayerJumpState : PlayerAirState
    {

        public PlayerJumpState(Enum id, XMachineActor<PlayerContext> actor) : base(id: id, actor: actor)
        {
        }
        internal override void Entry()
        {
            InvokeAction(action: PlayerAction.ForceJump);
        }
        internal override bool Guard()
        {
            return actor.objectChecker.isGrounded;
        }
    }
}
