using ND25.Core.XMachine;
using System;
using System.Collections.Generic;
namespace ND25.Component.Character.Player.States
{
    public class PlayerIdleState : XMachineState<PlayerContext>
    {

        public PlayerIdleState(Enum id, PlayerActor actor) : base(id: id, actor: actor)
        {
        }


        public override void Entry()
        {
        }
        public override void FixedUpdate()
        {
            InvokeAction(action: PlayerAction.SyncRigidContextAction);
            InvokeAction(action: PlayerAction.CheckNotInGroundAction);
            InvokeAction(action: PlayerAction.MoveTransitionAction);
        }
        public override void Update()
        {
            InvokeAction(action: PlayerAction.XInputListenAction);
            InvokeAction(action: PlayerAction.JumpInputListenAction);
        }
        public override void Exit()
        {
        }
    }
}
