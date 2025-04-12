using ND25.Core.XMachine;
namespace ND25.Gameplay.Character.Player
{
    public class PlayerAction
    {
        public enum DataKey
        {
            Transtion,
            JumpForce
        }

        public static readonly XMachineAction SyncVelocityContextAction = new XMachineAction(type: "SyncRigidContextAction");

        public static readonly XMachineAction MoveAction = new XMachineAction(type: "XInputListenAction");
        public static readonly XMachineAction ForceJump = new XMachineAction(type: "ForceJump");

        public static readonly XMachineAction CheckFallGroundAction = new XMachineAction(type: "CheckFallGroundAction");
        public static readonly XMachineAction CheckNotInGroundAction = new XMachineAction(type: "CheckNotInGroundAction");
    }
}
