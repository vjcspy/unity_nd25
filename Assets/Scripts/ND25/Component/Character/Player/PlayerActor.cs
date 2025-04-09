using ND25.Component.Character.Player.Effects;
using ND25.Component.Character.Player.States;
using ND25.Core.XMachine;
using System;
namespace ND25.Component.Character.Player
{
    public class PlayerActor : XMachineActor<PlayerContext>
    {

        protected override PlayerContext ConfigureInitialContext()
        {
            return new PlayerContext();
        }
        protected override XMachineState<PlayerContext>[] ConfigureMachineStates()
        {
            return new XMachineState<PlayerContext>[]
            {
                new PlayerIdleState(id: PlayerState.Idle, machine: machine), new PlayerMoveState(id: PlayerState.Idle, machine: machine), new PlayerPrimaryAttackState(id: PlayerState.Idle, machine: machine)
            };
        }
        protected override XMachineEffect<PlayerContext>[] ConfigureMachineEffects()
        {
            return new XMachineEffect<PlayerContext>[]
            {
                new PlayerCommonEffect(machine: machine)
            };
        }
        protected override Enum ConfigureInitialStateId()
        {
            return PlayerState.Idle;
        }
    }
}
