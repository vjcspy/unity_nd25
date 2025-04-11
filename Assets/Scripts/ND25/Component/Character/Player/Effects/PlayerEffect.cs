using ND25.Core.XMachine;
using ND25.Util.Common.Enum;
using UnityEngine;
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
            PlayerActor playerActor = (PlayerActor)actor;
            playerActor.machine.SetContext(contextUpdater: playerContext =>
            {
                playerContext.xVelocity = Direction.ConvertToXDirection(playerActor.rb.linearVelocityX);
                playerContext.yVelocity = Direction.ConvertToYDirection(playerActor.rb.linearVelocityY);

                return playerContext;
            });
        }

        [XMachineSubscribe(PlayerAction.FORCE_JUMP_ACTION_TYPE)]
        public void ForceJump(XMachineAction _)
        {
            PlayerActor playerActor = (PlayerActor)actor;
            playerActor.machine.SetContext(contextUpdater: context =>
            {
                context.lastJumpTime = Time.time;
                return context;
            });
            playerActor.ForceJump();
        }

        [XMachineSubscribe(PlayerAction.MOVE_TRANSITION_ACTION_TYPE)]
        public void MoveTransition(XMachineAction _)
        {
            PlayerActor playerActor = (PlayerActor)actor;
            playerActor.machine.Transition(toStateId: playerActor.machine.GetContextValue().xInput != 0 ? PlayerState.Move : PlayerState.Idle);
        }
    }
}
