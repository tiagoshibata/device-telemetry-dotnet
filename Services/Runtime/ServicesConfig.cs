// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime
{
    public interface IServicesConfig
    {
        string RulesTemplatesFolder { get; set; }
        string StorageConnString { get; set; }
        string KeyValueStorageApiUrl { get; set; }
        int KeyValueStorageApiTimeout { get; set; }
    }

    public class ServicesConfig : IServicesConfig
    {
        public string RulesTemplatesFolder { get; set; }
        public string StorageConnString { get; set; }
        public string KeyValueStorageApiUrl { get; set; }
        public int KeyValueStorageApiTimeout { get; set; }
    }
}
