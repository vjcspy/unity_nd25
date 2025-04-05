using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace ND25.Core.ReactiveMachine
{
    public class ReactiveMachineAction
    {
        public ReactiveMachineAction(string type, Dictionary<string, object> payload = null)
        {
            this.type = type;
            this.payload = payload;
        }

        public ReactiveMachineAction(Enum type, Dictionary<string, object> payload = null)
        {
            this.type = type.ToString();
            this.payload = payload;
        }

        public static ReactiveMachineAction Create(Enum type, Dictionary<string, object> payload = null)
        {
            return new ReactiveMachineAction(type, payload);
        }

        [JsonProperty("param")] public Dictionary<string, object> payload { get; }

        [JsonProperty("type")] public string type { get; }
    }

    public class ReactiveMachineCoreAction
    {
        public static readonly ReactiveMachineAction Empty = new ReactiveMachineAction("EMPTY_ACTION");
    }
}