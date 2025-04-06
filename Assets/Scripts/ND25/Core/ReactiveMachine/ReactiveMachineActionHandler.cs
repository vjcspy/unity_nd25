using R3;
namespace ND25.Core.ReactiveMachine
{
    public delegate Observable<ReactiveMachineAction> ReactiveMachineActionHandler(Observable<ReactiveMachineAction> upstream);
}
