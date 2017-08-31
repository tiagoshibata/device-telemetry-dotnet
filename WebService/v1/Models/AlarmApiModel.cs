// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Alarm = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Alarm;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models
{
    public class AlarmApiModel
    {
        private const string dateFormat = "yyyy-MM-dd'T'HH:mm:sszzz";
        private DateTimeOffset dateCreated;
        private DateTimeOffset dateModified;

        [JsonProperty(PropertyName = "ETag")]
        public string ETag { get; set; }

        [JsonProperty(PropertyName = "Id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "DateCreated")]
        public string DateCreated
        {
            get
            {
                return this.dateCreated.ToString(dateFormat);
            }
        }

        [JsonProperty(PropertyName = "DateModified")]
        public string DateModified
        {
            get
            {
                return this.dateModified.ToString(dateFormat);
            }
        }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "GroupId")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "DeviceId")]
        public string DeviceId { get; set; }

        [JsonProperty(PropertyName = "Status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "Rule")]
        public AlarmRuleApiModel Rule { get; set; }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public Dictionary<string, string> Metadata;

        public AlarmApiModel(Alarm alarm)
        {
            if (alarm != null)
            {
                this.ETag = alarm.ETag;
                this.Id = alarm.Id;
                this.dateCreated = alarm.DateCreated;
                this.dateModified = alarm.DateModified;
                this.Description = alarm.Description;
                this.GroupId = alarm.GroupId;
                this.DeviceId = alarm.DeviceId;
                this.Status = alarm.Status;
                this.Rule = new AlarmRuleApiModel(
                    alarm.RuleId,
                    alarm.RuleSeverity,
                    alarm.RuleDescription);

                this.Metadata = new Dictionary<string, string>
                {
                    { "$type", $"Alarm;" + Version.Number },
                    { "$uri", "/" + Version.Path + "/alarms/" + this.Id }
                };
            }
        }
    }
}
