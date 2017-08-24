// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime
{
    public class StorageConfig
    {
        public string DocumentDbDatabase { get; set; }
        public string DocumentDbCollection { get; set; }

        public StorageConfig(
            string documentDbDatabase,
            string documentDbCollection)
        {
            this.DocumentDbDatabase = documentDbDatabase;
            if (this.DocumentDbDatabase == null || this.DocumentDbDatabase.Length == 0)
            {
                throw new Exception("DocumentDb database name is empty in configuration");
            }

            this.DocumentDbCollection = documentDbCollection;
            if (this.DocumentDbCollection == null || this.DocumentDbCollection.Length == 0)
            {
                throw new Exception("DocumentDb collection name is empty in configuration");
            }
        }
    }
}
