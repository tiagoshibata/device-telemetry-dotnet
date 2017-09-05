// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models
{
    public class Condition
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public Condition()
        {
            this.Field = string.Empty;
            this.Operator = string.Empty;
            this.Value = string.Empty;
        }

        public Condition(
            string field,
            string @operator,
            string value)
        {
            this.Field = field;
            this.Operator = @operator;
            this.Value = value;
        }
    }
}
