using ND25.Core.XMachine;
using System;
using System.Collections.Generic;
namespace ND25.Component.Character.Player.States
{
    public class PlayerMoveState : XMachineState<PlayerContext>
    {
        public PlayerMoveState(Enum id, PlayerActor actor) : base(id: id, actor: actor)
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
            InvokeAction(action: PlayerAction.SyncRigidContextAction);
            InvokeAction(action: PlayerAction.MoveHandlerAction);
            InvokeAction(action: PlayerAction.MoveTransitionAction);
        }
        public override void Update()
        {
            InvokeAction(action: PlayerAction.JumpHandlerAction);
        }
        public override void Exit()
        {
        }
    }
}
