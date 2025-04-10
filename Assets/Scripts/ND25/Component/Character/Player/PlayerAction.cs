using ND25.Core.XMachine;
namespace ND25.Component.Character.Player
{
    public class PlayerAction
    {
        public const string MOVE_HANDLER = "MOVE_HANDLER";
        public const string JUMP_HANDLER = "JUMP_HANDLER";
        public const string SYNC_RIGID_CONTEXT = "SYNC_RIGID_CONTEXT";

        public static readonly XMachineAction MoveHandlerAction = new XMachineAction(type: MOVE_HANDLER);
        public static readonly XMachineAction JumpHandlerAction = new XMachineAction(type: JUMP_HANDLER);
        public static readonly XMachineAction SyncRigidContextAction = new XMachineAction(type: SYNC_RIGID_CONTEXT);
    }
}
