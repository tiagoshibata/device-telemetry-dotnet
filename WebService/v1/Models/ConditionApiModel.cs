// Copyright (c) Microsoft. All rights reserved.

using Newtonsoft.Json;
using Condition = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Condition;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models
{
    public class ConditionApiModel
    {
        [JsonProperty(PropertyName = "Field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "Operator")]
        public string Operator { get; set; }

        [JsonProperty(PropertyName = "Value")]
        public string Value { get; set; }

        public ConditionApiModel(
            string field,
            string @operator,
            string value)
        {
            this.Field = field;
            this.Operator = @operator;
            this.Value = value;
        }

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
            return new Condition(
                this.Field,
                this.Operator,
                this.Value);
        }
    }
}
