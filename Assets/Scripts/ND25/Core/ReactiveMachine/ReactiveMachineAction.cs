using System.Collections.Generic;
using Newtonsoft.Json;

namespace ND25.Core.ReactiveMachine
{
    public class ReactiveMachineAction
    {
        public ReactiveMachineAction(string type, Dictionary<string, object> payload = null)
        {
            this.type = type;
            this.payload = payload;
        }

        [JsonProperty("param")] public Dictionary<string, object> payload { get; }

        [JsonProperty("type")] public string type { get; }
    }

    public class ReactiveMachineCoreAction
    {
        public static readonly ReactiveMachineAction Empty = new("EMPTY_ACTION");
    }
}