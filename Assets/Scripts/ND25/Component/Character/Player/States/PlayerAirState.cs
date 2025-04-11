using ND25.Core.XMachine;
using System;
namespace ND25.Component.Character.Player.States
{
    public class PlayerAirState : XMachineState<PlayerContext>
    {

        public PlayerAirState(Enum id, XMachineActor<PlayerContext> actor) : base(id: id, actor: actor)
        {
        }
        internal override void FixedUpdate()
        {
            InvokeStateAction(action: PlayerAction.SyncVelocityContextAction);
            InvokeStateAction(action: PlayerAction.MoveAction);
            InvokeStateAction(action: PlayerAction.CheckFallGroundAction);
        }
    }
}
