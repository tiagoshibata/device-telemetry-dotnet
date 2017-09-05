// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;
using Rule = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Rule;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models
{
    public class RuleListApiModel
    {
        private List<RuleApiModel> items;

        [JsonProperty(PropertyName = "Items")]
        public List<RuleApiModel> Items
        {
            get { return this.items; }
        }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public IDictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "RuleList;" + Version.Number },
            { "$uri", "/" + Version.Path + "/rules" },
        };

        public RuleListApiModel(List<Rule> rules)
        {
            this.items = new List<RuleApiModel>();
            if (rules != null)
            {
                foreach (Rule rule in rules)
                {
                    this.items.Add(new RuleApiModel(rule));
                }
            }
        }
    }
}
