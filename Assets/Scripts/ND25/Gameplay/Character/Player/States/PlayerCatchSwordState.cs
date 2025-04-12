using ND25.Core.XMachine;
using System;
namespace ND25.Gameplay.Character.Player.States
{
    public class PlayerCatchSwordState : XMachineState<PlayerContext>
    {

        public PlayerCatchSwordState(Enum id, XMachineActor<PlayerContext> actor) : base(id: id, actor: actor)
        {
        }
    }
}
