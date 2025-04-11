using ND25.Core.XMachine;
using R3;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerPrimaryAttackEffect: XMachineEffect<PlayerContext>
    {

        public PlayerPrimaryAttackEffect(XMachineActor<PlayerContext> actor) : base(actor)
        {
        }

        [XMachineEffect]
        public XMachineActionHandler PrimaryAttackHandler()
        {
            return upstream => upstream.OfAction(xAction: PlayerAction.PrimaryAttackListenAction)
                .Select(selector: _ =>
                {
                    PlayerActor playerActor = (PlayerActor)actor;
                    if (!playerActor.pcControls.GamePlay.PrimaryAttack.triggered)
                    {
                        return XMachineAction.Empty;
                    }



                    return XMachineAction.Empty;
                });
        }
    }
}
