using ND25.Core.XMachine;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace ND25.Component.Character.Player.States
{
    public class PlayerIdleState : XMachineState<PlayerContext>
    {

        public PlayerIdleState(Enum id, PlayerActor actor) : base(id, actor)
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
            InvokeAction(PlayerAction.MoveHandler);
        }
        public override void Exit()
        {
        }
    }
}
