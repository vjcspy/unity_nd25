using R3;
using System;
using System.Linq;
using UnityEngine;
namespace ND25.Core.XMachine
{
    public static class ObservableExtensions
    {
        public static Observable<XMachineAction> OfAction(
            this Observable<XMachineAction> source,
            params string[] actions)
        {
            return source.Where(
                predicate: action => actions
                    .Contains(value: action.type)
            );
        }
        public static Observable<XMachineAction> OfAction(
            this Observable<XMachineAction> source,
            params Enum[] actions
        )
        {
            string[] actionStrings = actions
                .Select(selector: action => action.ToString())
                .ToArray();
            return source.Where(
                predicate: action => actionStrings
                    .Contains(value: action.type)
            );
        }

        public static Observable<XMachineAction> OfAction(
            this Observable<XMachineAction> source,
            params XMachineAction[] actions
        )
        {
            string[] actionStrings = actions
                .Select(selector: action => action.type)
                .ToArray();
            return source.Where(
                predicate: action => actionStrings
                    .Contains(value: action.type)
            );
        }
    }
}
