using ND25.Core.XMachine;
using UnityEngine;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerEffect : XMachineEffect<PlayerContext>
    {
        public PlayerEffect(XMachineActor<PlayerContext> actor) : base(actor: actor)
        {
        }

        [XMachineSubscribe(PlayerAction.SYNC_RIGID_CONTEXT)]
        public void SyncContext(XMachineAction _)
        {
            PlayerActor playerActor = (PlayerActor)actor;
            playerActor.machine.SetContext(contextUpdater: playerContext =>
            {
                playerContext.xVelocity = playerActor.rb.linearVelocityX;
                playerContext.yVelocity = playerActor.rb.linearVelocityY;

                return playerContext;
            });
        }

        [XMachineSubscribe(PlayerAction.FORCE_JUMP)]
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

        [XMachineSubscribe(PlayerAction.MOVE_TRANSITION)]
        public void MoveTransition(XMachineAction _)
        {
            PlayerActor playerActor = (PlayerActor)actor;
            playerActor.machine.Transition(toStateId: playerActor.machine.GetContextValue().xInput != 0 ? PlayerState.Move : PlayerState.Idle);
        }
    }
}
