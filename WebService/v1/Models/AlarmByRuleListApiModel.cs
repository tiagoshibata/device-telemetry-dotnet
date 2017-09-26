// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models
{
    public class AlarmByRuleListApiModel
    {
        private enum Status
        {
            // ReSharper disable once InconsistentNaming
            closed = 1,
            // ReSharper disable once InconsistentNaming
            acknowledged = 2,
            // ReSharper disable once InconsistentNaming
            open = 3
        }

        private List<AlarmByRuleApiModel> items;

        [JsonProperty(PropertyName = "Items")]
        public List<AlarmByRuleApiModel> Items
        {
            get { return this.items; }

            private set { }
        }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public Dictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", $"AlarmByRuleList;" + Version.NUMBER },
            { "$uri", "/" + Version.PATH + "/alarmsbyrule" }
        };

        public AlarmByRuleListApiModel(List<Alarm> alarms)
        {
            this.items = new List<AlarmByRuleApiModel>();
            if (alarms != null)
            {
                Dictionary<string, AlarmByRuleApiModel> dictionary =
                    new Dictionary<string, AlarmByRuleApiModel>();
                foreach (Alarm alarm in alarms)
                {
                    string id = alarm.RuleId;
                    if (!dictionary.ContainsKey(id))
                    {
                        dictionary.Add(id, new AlarmByRuleApiModel(1, alarm.Status, alarm.DateModified, alarm));
                    }
                    else
                    {
                        AlarmByRuleApiModel alarmByRule;
                        if (dictionary.TryGetValue(id, out alarmByRule))
                        {
                            alarmByRule.Count = alarmByRule.Count++;

                            DateTimeOffset created = alarm.DateCreated;

                            // We only want to update the created date with the latest timestamp
                            if (DateTimeOffset.Parse(alarmByRule.Created).CompareTo(created) < 0)
                            {
                                alarmByRule.Created = created.ToString();
                            }

                            string savedStatus = alarmByRule.Status;
                            string newStatus = alarm.Status;

                            Status savedStatusCode = (Status) Enum.Parse(typeof(Status), savedStatus);
                            Status newStatusCode = (Status) Enum.Parse(typeof(Status), newStatus);

                            // The status value that gets set in case of multiple alarms
                            // with different status follows priority order:
                            // 1) Open, 2) Acknowledged and 3) Closed
                            if (savedStatusCode < newStatusCode)
                            {
                                alarmByRule.Status = newStatus;
                            }

                            dictionary[id] = alarmByRule;
                        }
                    }
                }

                List<AlarmByRuleApiModel> values = dictionary.Values.ToList();
                foreach (AlarmByRuleApiModel value in values)
                {
                    this.items.Add(value);
                }
            }
        }
    }
}
