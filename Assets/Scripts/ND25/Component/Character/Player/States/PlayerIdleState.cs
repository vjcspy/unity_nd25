using ND25.Core.XMachine;
using System;
namespace ND25.Component.Character.Player.States
{
    public class PlayerIdleState : XMachineState<PlayerContext>
    {

        public PlayerIdleState(Enum id, PlayerActor actor) : base(id: id, actor: actor)
        {
        }

        internal override bool Guard()
        {
            return actor.objectChecker.isGrounded;
        }
        internal override void FixedUpdate()
        {
            InvokeStateAction(action: PlayerAction.SyncVelocityContextAction);
            InvokeStateAction(action: PlayerAction.CheckNotInGroundAction);
        }
        internal override void Update()
        {
            InvokeStateAction(action: PlayerAction.MoveAction.SetBool(PlayerAction.DataKey.Transtion, true));
        }
    }
}
