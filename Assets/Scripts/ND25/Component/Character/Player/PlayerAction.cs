using ND25.Core.XMachine;
using UnityEngine;
namespace ND25.Component.Character.Player
{
    public class PlayerAction
    {
        public const string XINPUT_LISTEN_ACTION_TYPE = "XINPUT_LISTEN_ACTION_TYPE";
        public const string MOVE_TRANSITION_ACTION_TYPE = "MOVE_TRANSITION_ACTION_TYPE";
        public const string JUMP_INPUT_LISTEN_ACTION_TYPE = "JUMP_INPUT_LISTEN_ACTION_TYPE";
        public const string SYNC_RIGID_CONTEXT_ACTION_TYPE = "SYNC_RIGID_CONTEXT_ACTION_TYPE";
        public const string FORCE_JUMP_ACTION_TYPE = "FORCE_JUMP_ACTION_TYPE";

        public static readonly XMachineAction SyncRigidContextAction = new XMachineAction(type: SYNC_RIGID_CONTEXT_ACTION_TYPE);

        public static readonly XMachineAction XInputListenAction = new XMachineAction(type: XINPUT_LISTEN_ACTION_TYPE);
        public static readonly XMachineAction MoveTransitionAction = new XMachineAction(type: MOVE_TRANSITION_ACTION_TYPE);

        public static readonly XMachineAction JumpInputListenAction = new XMachineAction(type: JUMP_INPUT_LISTEN_ACTION_TYPE);
        public static readonly XMachineAction ForceJumpAction = new XMachineAction(type: FORCE_JUMP_ACTION_TYPE);
        public static readonly XMachineAction CheckFallGroundAction = new XMachineAction(type: "CheckFallGroundAction");
        public static readonly XMachineAction CheckNotInGroundAction = new XMachineAction(type: "CheckNotInGroundAction");
    }
}
