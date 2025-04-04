using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ND25.Core.ReactiveMachine
{
    public class ReactiveMachineStateConfigConverter : JsonConverter<ReactiveMachineStateConfig>
    {
        public override ReactiveMachineStateConfig ReadJson(
            JsonReader reader,
            Type objectType,
            ReactiveMachineStateConfig existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            JObject jo = JObject.Load(reader);
            ReactiveMachineStateConfig result = new ReactiveMachineStateConfig();

            // Normalize entry
            JToken entryToken = jo["entry"];
            result.entry = ParseActions(entryToken);

            // Normalize invoke
            JToken invokeToken = jo["invoke"];
            result.invoke = ParseActions(invokeToken, true);

            // Normalize exit
            JToken exitToken = jo["exit"];
            result.exit = ParseActions(exitToken);

            // Handle 'on'
            result.on = jo["on"]
                ?.ToObject<Dictionary<string, List<StateTransition>>>();

            return result;
        }

        List<ReactiveMachineAction> ParseActions(JToken token, bool isInvokeConfig = false)
        {
            if (isInvokeConfig)
            {
                return token switch
                {
                    null => new List<ReactiveMachineAction>(),
                    JArray actions => actions
                        .Select(
                            item => new ReactiveMachineAction(
                                item["src"]
                                    ?.ToString(),
                                item["input"]
                                    ?.ToObject<Dictionary<string, object>>()
                            )
                        )
                        .ToList(),
                    JObject action => new List<ReactiveMachineAction>
                    {
                        new ReactiveMachineAction(
                            action["src"]
                                ?.ToString(),
                            action["input"]
                                ?.ToObject<Dictionary<string, object>>()
                        ),
                    },
                    _ => new List<ReactiveMachineAction>(),
                };
            }

            return token switch
            {
                null => new List<ReactiveMachineAction>(),
                JArray actions => actions
                    .Select(
                        item => new ReactiveMachineAction(
                            item["type"]
                                ?.ToString(),
                            item["params"]
                                ?.ToObject<Dictionary<string, object>>()
                        )
                    )
                    .ToList(),
                JObject action => new List<ReactiveMachineAction>
                {
                    new ReactiveMachineAction(
                        action["type"]
                            ?.ToString(),
                        action["params"]
                            ?.ToObject<Dictionary<string, object>>()
                    ),
                },
                _ => new List<ReactiveMachineAction>(),
            };
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
