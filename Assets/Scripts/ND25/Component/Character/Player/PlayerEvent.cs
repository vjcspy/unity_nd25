using ND25.Core.XMachine;
using System.Collections.Generic;
namespace ND25.Component.Character.Player
{

    public class PlayerEvent
    {
        public enum InputEvent
        {
            TRIGGER_JUMP,
            TRIGGER_PRIMARY_ATTACK,
            TRIGGER_SECONDARY_ATTACK,
            TRIGGER_DODGE,
            TRIGGER_INTERACT,
            TRIGGER_PAUSE,
            TRIGGER_MENU,
            TRIGGER_CANCEL,
            TRIGGER_MOVE_LEFT,
            TRIGGER_MOVE_RIGHT,
            TRIGGER_MOVE_UP,
            TRIGGER_MOVE_DOWN
        }

        static Dictionary<InputEvent, XMachineAction> inputEventActions = new Dictionary<InputEvent, XMachineAction>
        {
            { InputEvent.TRIGGER_JUMP, PlayerAction.PrimaryAttackTriggerAction },
            // { InputEvent.TRIGGER_PRIMARY_ATTACK, new XMachineAction("TRIGGER_PRIMARY_ATTACK") },
            // { InputEvent.TRIGGER_SECONDARY_ATTACK, new XMachineAction("TRIGGER_SECONDARY_ATTACK") },
            // { InputEvent.TRIGGER_DODGE, new XMachineAction("TRIGGER_DODGE") },
            // { InputEvent.TRIGGER_INTERACT, new XMachineAction("TRIGGER_INTERACT") },
            // { InputEvent.TRIGGER_PAUSE, new XMachineAction("TRIGGER_PAUSE") },
            // { InputEvent.TRIGGER_MENU, new XMachineAction("TRIGGER_MENU") },
            // { InputEvent.TRIGGER_CANCEL, new XMachineAction("TRIGGER_CANCEL") },
            // { InputEvent.TRIGGER_MOVE_LEFT, new XMachineAction("TRIGGER_MOVE_LEFT") },
            // { InputEvent.TRIGGER_MOVE_RIGHT, new XMachineAction("TRIGGER_MOVE_RIGHT") },
            // { InputEvent.TRIGGER_MOVE_UP, new XMachineAction("TRIGGER_MOVE_UP") },
            // { InputEvent.TRIGGER_MOVE_DOWN, new XMachineAction("TRIGGER_MOVE_DOWN") }
        };
    }
}
