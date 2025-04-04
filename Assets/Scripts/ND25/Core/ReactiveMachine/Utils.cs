using R3;
using System;
using System.Linq;
namespace ND25.Core.ReactiveMachine
{
    public static class ObservableExtensions
    {
        public static Observable<ReactiveMachineAction> OfAction(
            this Observable<ReactiveMachineAction> source,
            params string[] actions
        )
        {
            return source.Where(
                action => actions
                    .Contains(action.type)
            );
        }
        public static Observable<ReactiveMachineAction> OfAction(
            this Observable<ReactiveMachineAction> source,
            params Enum[] actions
        )
        {
            string[] actionStrings = actions
                .Select(action => action.ToString())
                .ToArray();
            return source.Where(
                action => actionStrings
                    .Contains(action.type)
            );
        }
    }


}
