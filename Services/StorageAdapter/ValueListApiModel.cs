// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.StorageAdapter
{
    public class ValueListApiModel
    {
        public List<ValueApiModel> Items { get; set; }

        [JsonProperty("$metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        public ValueListApiModel()
        {
            this.Items = new List<ValueApiModel>();
        }
    }
}
