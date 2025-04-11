using ND25.Core.XMachine;
using System;
namespace ND25.Component.Character.Player.States
{
    public class PlayerAirState : XMachineState<PlayerContext>
    {

        public PlayerAirState(Enum id, XMachineActor<PlayerContext> actor) : base(id: id, actor: actor)
        {
        }
        internal override void Entry()
        {
        }
        internal override void FixedUpdate()
        {
            InvokeAction(action: PlayerAction.SyncVelocityContextAction);
            InvokeAction(action: PlayerAction.MoveAction);
            InvokeAction(action: PlayerAction.CheckFallGroundAction);
        }
        internal override void Update()
        {
        }
        internal override void Exit()
        {
        }
    }
}
