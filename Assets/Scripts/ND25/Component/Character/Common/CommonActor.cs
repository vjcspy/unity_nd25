using ND25.Core.ReactiveMachine;
using ND25.Core.Utils;
using R3;
using System;
using UnityEngine;
namespace ND25.Component.Character.Common
{
    public static class CommonActor
    {
        public static ReactiveMachineActionHandler UpdateAnimatorParams<T>(AnimatorParamMap<T> paramMap) where T : Enum
        {
            return upstream => upstream
                .OfAction("UpdateAnimatorParams")
                .Select(
                    action =>
                    {

                        if (action.payload == null)
                        {
                            return ReactiveMachineCoreAction.Empty;
                        }

                        foreach ((string keyString, object value) in action.payload)
                        {
                            // Debug.Log("Update animator key: " + keyString);
                            switch (value)
                            {
                                case bool boolVal:
                                    paramMap.UpdateParam(keyString, boolVal);
                                    break;
                                case float floatVal:
                                    paramMap.UpdateParam(keyString, floatVal);
                                    break;
                                case double doubleVal:
                                    paramMap.UpdateParam(keyString, (float)doubleVal);
                                    break;
                                default:
                                    Debug.Log("Unsupported type: " + value.GetType());
                                    break;
                            }
                        }

                        return ReactiveMachineCoreAction.Empty;
                    }
                );
        }
    }
}
