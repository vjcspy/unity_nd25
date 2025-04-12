using ND25.Core.XMachine;
using System;
namespace ND25.Gameplay.Character.Player.States
{
    public class PlayerAimSwordState : XMachineState<PlayerContext>
    {
        public PlayerAimSwordState(Enum id, XMachineActor<PlayerContext> actor) : base(id, actor)
        {
        }
    }
}
