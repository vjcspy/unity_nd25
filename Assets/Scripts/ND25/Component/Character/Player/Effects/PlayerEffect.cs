using ND25.Core.XMachine;
using R3;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerEffect : XMachineEffect<PlayerContext>
    {
        public PlayerEffect(XMachineActor<PlayerContext> actor) : base(actor: actor)
        {
        }
        [XMachineEffect]
        public XMachineActionHandler SyncContext()
        {
            return upstream => upstream.OfAction(actions: new[]
            {
                PlayerAction.SyncRigidContextAction
            }).Select(selector: _ =>
            {
                PlayerActor playerActor = (PlayerActor)actor;
                playerActor.machine.SetContext(contextUpdater: playerContext =>
                {
                    playerContext.xVelocity = playerActor.rb.linearVelocityX;
                    playerContext.yVelocity = playerActor.rb.linearVelocityY;
                    return playerContext;
                });
                return XMachineAction.Empty;
            });
        }
    }
}
