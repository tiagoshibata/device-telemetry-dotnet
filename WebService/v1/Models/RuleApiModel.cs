// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models
{
    public class RuleApiModel
    {
        private const string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:sszzz";

        [JsonProperty(PropertyName = "ETag")]
        public string ETag { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "DateCreated")]
        public string DateCreated { get; set; } = DateTimeOffset.UtcNow.ToString(DATE_FORMAT);

        [JsonProperty(PropertyName = "DateModified")]
        public string DateModified { get; set; } = DateTimeOffset.UtcNow.ToString(DATE_FORMAT);

        [JsonProperty(PropertyName = "Enabled")]
        public bool Enabled { get; set; } = false;

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "GroupId")]
        public string GroupId { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Severity")]
        public string Severity { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Conditions")]
        public List<ConditionApiModel> Conditions { get; set; } = new List<ConditionApiModel>();

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public IDictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "Rule;" + Version.NUMBER },
            { "$uri", "/" + Version.PATH + "/rules/" + this.Id }
        };

        public RuleApiModel() { }

        public RuleApiModel(Rule rule)
        {
            if (rule != null)
            {
                this.ETag = rule.ETag;
                this.Id = rule.Id;
                this.Name = rule.Name;
                this.DateCreated = rule.DateCreated;
                this.DateModified = rule.DateModified;
                this.Enabled = rule.Enabled;
                this.Description = rule.Description;
                this.GroupId = rule.GroupId;
                this.Severity = rule.Severity;

                foreach (Condition condition in rule.Conditions)
                {
                    this.Conditions.Add(new ConditionApiModel(condition));
                }
            }
        }

        public Rule ToServiceModel()
        {
            List<Condition> conditions = new List<Condition>();
            foreach (ConditionApiModel condition in this.Conditions)
            {
                conditions.Add(condition.ToServiceModel());
            }

            return new Rule()
            {
                ETag = this.ETag,
                Id = this.Id,
                Name = this.Name,
                DateCreated = this.DateCreated,
                DateModified = this.DateModified,
                Enabled = this.Enabled,
                Description = this.Description,
                GroupId = this.GroupId,
                Severity = this.Severity,
                Conditions = conditions
            };
        }
    }
}
