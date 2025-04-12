using ND25.Core.XMachine;
namespace ND25.Gameplay.Character.Player.Effects
{
    public class PlayerPrimaryAttackEffect : XMachineEffect<PlayerContext>
    {

        public PlayerPrimaryAttackEffect(PlayerActor actor) : base(actor: actor)
        {
        }
    }
}
