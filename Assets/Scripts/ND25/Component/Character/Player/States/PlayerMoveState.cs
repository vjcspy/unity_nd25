using ND25.Core.XMachine;
using System;
using System.Collections.Generic;
namespace ND25.Component.Character.Player.States
{
    public class PlayerMoveState : XMachineState<PlayerContext>
    {
        public PlayerMoveState(Enum id, XMachine<PlayerContext> machine) : base(id, machine)
        {
        }

        public override HashSet<int> allowedEvents
        {
            get;
        } = new HashSet<int>();

        public override void Entry()
        {
            throw new NotImplementedException();
        }
        public override void Update()
        {
            throw new NotImplementedException();
        }
        public override void Exit()
        {
            throw new NotImplementedException();
        }
    }
}
