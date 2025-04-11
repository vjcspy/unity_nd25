using ND25.Core.XMachine;
using ND25.Util.Common.Enum;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerEffect : XMachineEffect<PlayerContext>
    {
        public PlayerEffect(XMachineActor<PlayerContext> actor) : base(actor: actor)
        {
        }

        [XMachineSubscribe(PlayerAction.SYNC_RIGID_CONTEXT_ACTION_TYPE)]
        public void SyncContext(XMachineAction _)
        {
            actor.machine.SetContext(contextUpdater: playerContext =>
            {
                playerContext.xVelocity = Direction.ConvertToXDirection(velocity: actor.rb.linearVelocityX);
                playerContext.yVelocity = Direction.ConvertToYDirection(velocity: actor.rb.linearVelocityY);
            });
        }

        [XMachineSubscribe(PlayerAction.FORCE_JUMP_ACTION_TYPE)]
        public void ForceJump(XMachineAction _)
        {
            PlayerActor playerActor = (PlayerActor)actor;
            playerActor.ForceJump();
        }

        [XMachineSubscribe(PlayerAction.MOVE_TRANSITION_ACTION_TYPE)]
        public void MoveTransition(XMachineAction _)
        {
            actor.machine.Transition(toStateId: actor.machine.GetContextValue().xInput != 0 ? PlayerState.Move : PlayerState.Idle);
        }
    }
}
