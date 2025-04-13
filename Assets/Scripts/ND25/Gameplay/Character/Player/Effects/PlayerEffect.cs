using ND25.Core.XMachine;
using ND25.Util.Common.Enum;
using R3;
namespace ND25.Gameplay.Character.Player.Effects
{
    public class PlayerEffect : XMachineEffect<PlayerContext>
    {
        public PlayerEffect(XMachineActor<PlayerContext> actor) : base(actor: actor)
        {
        }

        [XMachineEffect]
        public XMachineActionHandler SyncContextHandler()
        {
            return upstream => upstream.OfAction(xAction: PlayerAction.SyncVelocityContextAction)
                .Select(selector: _ =>
                {
                    actor.machine.SetContext(contextUpdater: playerContext =>
                    {
                        // playerContext.xVelocity = Direction.ConvertToXDirection(velocity: actor.rb.linearVelocityX);
                        playerContext.yVelocityDirection = Direction.ConvertToYDirection(velocity: actor.rb.linearVelocityY);
                    });
                    return XMachineAction.Empty;
                });
        }
    }
}
