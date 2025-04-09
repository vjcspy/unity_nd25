using ND25.Core.XMachine;
using System;
using System.Collections.Generic;
namespace ND25.Component.Character.Player.States
{
    public class PlayerIdleState : XMachineState<PlayerContext>
    {

        public PlayerIdleState(Enum id, XMachine<PlayerContext> machine) : base(id, machine)
        {
        }

        public override HashSet<int> allowedEvents
        {
            get;
        } = new HashSet<int>();

        public override void Entry()
        {
        }
        public override void Update()
        {
        }
        public override void Exit()
        {
        }
    }
}
