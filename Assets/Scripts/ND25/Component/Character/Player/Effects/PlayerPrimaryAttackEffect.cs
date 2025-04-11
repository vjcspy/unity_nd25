using ND25.Core.XMachine;
using R3;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerPrimaryAttackEffect : XMachineEffect<PlayerContext>
    {

        public PlayerPrimaryAttackEffect(PlayerActor actor) : base(actor: actor)
        {
        }

        [XMachineEffect]
        public XMachineActionHandler PrimaryAttackHandler()
        {
            return upstream => upstream.OfAction(xAction: PlayerAction.PrimaryAttackTriggerAction)
                .Select(selector: _ =>
                {
                    actor.machine.Transition(toStateId: PlayerState.PrimaryAttack);

                    return XMachineAction.Empty;
                });
        }

    }
}
