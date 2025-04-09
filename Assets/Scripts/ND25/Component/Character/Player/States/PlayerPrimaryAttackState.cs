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
        }
        public override void FixedUpdate()
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
