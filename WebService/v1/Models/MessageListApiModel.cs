// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Models
{
    public class MessageListApiModel
    {
        private readonly List<MessageApiModel> items = new List<MessageApiModel>();
        private readonly List<string> properties = new List<string>();

        [JsonProperty(PropertyName = "Items")]
        public List<MessageApiModel> Items {
            get { return this.items; }
        }

        [JsonProperty(PropertyName = "Properties")]
        public List<string> Properties
        {
            get { return this.properties; }
        }

        [JsonProperty(PropertyName = "$metadata", Order = 1000)]
        public IDictionary<string, string> Metadata => new Dictionary<string, string>
        {
            { "$type", "MessageList;" + Version.Number },
            { "$uri", "/" + Version.Path + "/messages" },
        };

        public MessageListApiModel(MessageList data)
        {
            if (data == null) return;

            foreach (Message  message in data.Messages)
            {
                this.items.Add(new MessageApiModel(message));
            }

            foreach (string s in data.Properties)
            {
                this.properties.Add(s);
            }
        }
    }
}
