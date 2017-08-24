using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using System;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime
{
    public class StorageConfig
    {
        public string StorageType { get; set; }
        public string DocumentDbDatabase { get; set; }
        public string DocumentDbCollection { get; set; }

        public StorageConfig(
            string storageType,
            string documentDbConnString,
            string documentDbDatabase,
            string documentDbCollection)
        {
            this.StorageType = storageType;
            this.DocumentDbDatabase = documentDbDatabase;
            this.DocumentDbCollection = documentDbCollection;
        }
    }
}
