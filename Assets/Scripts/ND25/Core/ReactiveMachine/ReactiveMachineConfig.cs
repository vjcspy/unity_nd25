using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ND25.Core.ReactiveMachine
{
    public class ReactiveMachineStateConfigConverter : JsonConverter<ReactiveMachineStateConfig>
    {
        public override ReactiveMachineStateConfig ReadJson(JsonReader reader, Type objectType,
            ReactiveMachineStateConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var result = new ReactiveMachineStateConfig();

            // Normalize entry
            var entryToken = jo["entry"];
            result.entry = ParseActions(entryToken);

            // Normalize invoke
            var invokeList = new List<ReactiveMachineAction>();
            if (jo["invoke"] is JArray invokes)
                foreach (var item in invokes)
                {
                    var action = new ReactiveMachineAction
                    (
                        item["src"]?.ToString(),
                        item["input"]?.ToObject<Dictionary<string, object>>()
                    );
                    invokeList.Add(action);
                }

            result.invoke = invokeList;

            // Normalize exit
            var exitToken = jo["exit"];
            result.exit = ParseActions(exitToken);

            // Handle 'on'
            result.on = jo["on"]?.ToObject<Dictionary<string, List<StateTransition>>>();

            return result;
        }

        private List<ReactiveMachineAction> ParseActions(JToken token)
        {
            if (token == null) return new List<ReactiveMachineAction>();

            if (token is JArray actions)
            {
                var actionList = new List<ReactiveMachineAction>();
                foreach (var item in actions)
                {
                    var action = new ReactiveMachineAction
                    (
                        item["src"]?.ToString(),
                        item["input"]?.ToObject<Dictionary<string, object>>()
                    );
                    actionList.Add(action);
                }

                return actionList;
            }

            if (token.Type == JTokenType.Object)
                return new List<ReactiveMachineAction> { token.ToObject<ReactiveMachineAction>() };

            return new List<ReactiveMachineAction>();
        }

        public override void WriteJson(JsonWriter writer, ReactiveMachineStateConfig value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Writing not supported");
        }
    }


    public class ReactiveMachineConfig
    {
        [JsonProperty("id")] public string id { get; set; }

        [JsonProperty("initial")] public string initial { get; set; }

        [JsonProperty("context")] public Dictionary<string, object> context { get; set; }

        [JsonProperty("states")] public Dictionary<string, ReactiveMachineStateConfig> states { get; set; }
    }

    public class ReactiveMachineStateConfig
    {
        [JsonProperty("entry")] public List<ReactiveMachineAction> entry { get; set; }

        [JsonProperty("invoke")] public List<ReactiveMachineAction> invoke { get; set; }

        [JsonProperty("exit")] public List<ReactiveMachineAction> exit { get; set; }

        [JsonProperty("on")] public Dictionary<string, List<StateTransition>> on { get; set; }
    }

    public class StateTransition
    {
        [JsonProperty("target")] public string target { get; set; }

        [JsonProperty("actions")] public List<ReactiveMachineAction> actions { get; set; }
    }
}