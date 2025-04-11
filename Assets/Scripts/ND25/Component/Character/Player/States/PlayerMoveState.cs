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


        internal override void Entry()
        {
        }
        internal override void FixedUpdate()
        {
            InvokeAction(action: PlayerAction.SyncRigidContextAction);
            InvokeAction(action: PlayerAction.MoveTransitionAction);
            InvokeAction(action: PlayerAction.CheckNotInGroundAction);
        }
        internal override void Update()
        {
            InvokeAction(action: PlayerAction.XInputListenAction);
            InvokeAction(action: PlayerAction.JumpInputListenAction);
        }
        internal override void Exit()
        {
        }
    }
}
