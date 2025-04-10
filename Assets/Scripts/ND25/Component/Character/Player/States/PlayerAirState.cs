using ND25.Core.XMachine;
using System;
using System.Collections.Generic;
namespace ND25.Component.Character.Player.States
{
    public class PlayerAirState : XMachineState<PlayerContext>
    {

        public PlayerAirState(Enum id, XMachineActor<PlayerContext> actor) : base(id: id, actor: actor)
        {
        }
        public override HashSet<int> allowedEvents { get; } = new HashSet<int>();
        public override void Entry()
        {
        }
        public override void FixedUpdate()
        {
            InvokeAction(action: PlayerAction.SyncRigidContextAction);
            InvokeAction(action: PlayerAction.MoveHandlerAction);
        }
        public override void Update()
        {
        }
        public override void Exit()
        {
        }
    }
}
