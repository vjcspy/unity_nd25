using ND25.Core.XMachine;
using System;
using System.Collections.Generic;
namespace ND25.Component.Character.Player.States
{
    public class PlayerPrimaryAttackState : XMachineState<PlayerContext>
    {
        public PlayerPrimaryAttackState(Enum id, PlayerActor actor) : base(id, actor)
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
