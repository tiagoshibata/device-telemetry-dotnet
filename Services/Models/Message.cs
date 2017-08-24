// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models
{
    public class Message
    {
        public string DeviceId { get; set; }
        public DateTimeOffset Time { get; set; }
        public Object Data { get; set; }

        public Message()
        {
            this.DeviceId = string.Empty;
            this.Time = DateTimeOffset.UtcNow;
            this.Data = null;
        }

        public Message(
            string deviceId,
            long time,
            Object data)
        {
            this.DeviceId = deviceId;
            this.Time = DateTimeOffset.FromUnixTimeMilliseconds(time);
            this.Data = data;
        }
    }
}
