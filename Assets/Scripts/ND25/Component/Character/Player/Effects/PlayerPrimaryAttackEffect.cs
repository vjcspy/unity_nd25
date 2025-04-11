using ND25.Core.XMachine;
using R3;
namespace ND25.Component.Character.Player.Effects
{
    public class PlayerPrimaryAttackEffect : XMachineEffect<PlayerContext>
    {

        public PlayerPrimaryAttackEffect(PlayerActor actor) : base(actor: actor)
        {
        }
    }
}
