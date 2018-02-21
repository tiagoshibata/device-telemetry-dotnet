// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models
{
    public class ConditionApiModel
    {
        [JsonProperty(PropertyName = "Field")]
        public string Field { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Operator")]
        public string Operator { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "Value")]
        public string Value { get; set; } = string.Empty;

        public ConditionApiModel() { }

        public ConditionApiModel(Condition condition)
        {
            if (condition != null)
            {
                this.Field = condition.Field;
                this.Operator = condition.Operator;
                this.Value = condition.Value;
            }
        }

        public Condition ToServiceModel()
        {
            return new Condition()
            {
                Field = this.Field,
                Operator = this.Operator,
                Value = this.Value
            };
        }
    }
}
