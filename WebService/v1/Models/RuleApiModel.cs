// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;
using Condition = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Condition;
using Rule = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Rule;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models
{
    public class RuleApiModel
    {
        [JsonProperty(PropertyName = "ETag")]
        public string ETag { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "DateCreated")]
        public string DateCreated { get; set; }

        [JsonProperty(PropertyName = "DateModified")]
        public string DateModified { get; set; }

        [JsonProperty(PropertyName = "Enabled")]
        public bool Enabled { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "GroupId")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "Severity")]
        public string Severity { get; set; }

        [JsonProperty(PropertyName = "Conditions")]
        public List<ConditionApiModel> Conditions { get; set; }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public IDictionary<string, string> Metadata { get; set; }

        public RuleApiModel(
            string eTag,
            string id,
            string name,
            string dateCreated,
            string dateModified,
            bool enabled,
            string description,
            string groupId,
            string severity,
            List<ConditionApiModel> conditions)
        {
            this.ETag = eTag;
            this.Id = id;
            this.Name = name;
            this.DateCreated = dateCreated;
            this.DateModified = dateModified;
            this.Enabled = enabled;
            this.Description = description;
            this.GroupId = groupId;
            this.Severity = severity;
            this.Conditions = conditions;

            this.Metadata = new Dictionary<string, string>
            {
                { "$type", $"Rule;" + Version.Number },
                { "$uri", "/" + Version.Path + "/rules/" + this.Id }
            };
        }

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

                this.Conditions = new List<ConditionApiModel>();
                foreach (Condition condition in rule.Conditions)
                {
                    this.Conditions.Add(new ConditionApiModel(condition));
                }

                this.Metadata = new Dictionary<string, string>
                {
                    { "$type", $"Rule;" + Version.Number },
                    { "$uri", "/" + Version.Path + "/rules/" + this.Id }
                };
            }
        }

        public Rule ToRuleModel()
        {
            List<Condition> conditions = new List<Condition>();
            foreach (ConditionApiModel condition in this.Conditions)
            {
                conditions.Add(new Condition(
                    condition.Field,
                    condition.Operator,
                    condition.Value));
            }

            return new Rule(
                this.ETag,
                this.Id,
                this.Name,
                this.DateCreated,
                this.DateModified,
                this.Enabled,
                this.Description,
                this.GroupId,
                this.Severity,
                conditions);
        }
    }
}
