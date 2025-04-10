using ND25.Core.XMachine;
namespace ND25.Component.Character.Player
{
    public class PlayerAction
    {
        public const string MOVE_HANDLER = "MOVE_HANDLER";
        public const string MOVE_TRANSITION = "MOVE_TRANSITION";
        public const string JUMP_HANDLER = "JUMP_HANDLER";
        public const string SYNC_RIGID_CONTEXT = "SYNC_RIGID_CONTEXT";
        public const string FORCE_JUMP = "FORCE_JUMP";

        public static readonly XMachineAction MoveHandlerAction = new XMachineAction(type: MOVE_HANDLER);
        public static readonly XMachineAction MoveTransitionAction = new XMachineAction(type: MOVE_TRANSITION);
        public static readonly XMachineAction JumpHandlerAction = new XMachineAction(type: JUMP_HANDLER);
        public static readonly XMachineAction ForceJumpAction = new XMachineAction(type: FORCE_JUMP);
        public static readonly XMachineAction SyncRigidContextAction = new XMachineAction(type: SYNC_RIGID_CONTEXT);
    }
}
