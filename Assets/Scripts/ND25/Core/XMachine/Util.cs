using R3;
using System;
using System.Collections.Generic;
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
            params XMachineAction[] actions)
        {
            // Dùng HashSet để tăng tốc độ tìm kiếm
            var typeSet = new HashSet<string>(
                collection: actions.Select(selector: action => action.type)
            );
            // Debug.Log("OfAction: " + typeSet.Count);

            return source.Where(
                predicate: action => typeSet.Contains(item: action.type)
            );
        }

        public static Observable<XMachineAction> OfAction(
            this Observable<XMachineAction> source,
            XMachineAction xAction
        )
        {
            return source.Where(
                predicate: action => xAction.type == action.type
            );
        }
        public static Observable<XMachineAction> OfAction(
            this Observable<XMachineAction> source,
            HashSet<string> xActionType
        )
        {
            return source.Where(
                predicate: action => xActionType.Contains(value: action.type)
            );
        }
    }
}
