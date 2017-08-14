// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.Runtime
{
    public interface IConfig
    {
        /// <summary>Web service listening port</summary>
        int Port { get; }

        /// <summary>Service layer configuration</summary>
        IServicesConfig ServicesConfig { get; }
    }

    /// <summary>Web service configuration</summary>
    public class Config : IConfig
    {
        private const string ApplicationKey = "devicetelemetry:";
        private const string PortKey = ApplicationKey + "webservice_port";
        private const string RulesTemplatesFolderKey = ApplicationKey + "rules_templates_folder";

        private const string StorageKey = "documentdb:";
        private const string StorageConnStringKey = StorageKey + "connstring";

        private const string KeyValueStorageKey = "storageadapter:";
        private const string KeyValueStorageApiUrlKey = KeyValueStorageKey + "webservice_url";
        private const string KeyValueStorageApiTimeoutKey = KeyValueStorageKey + "webservice_timeout";

        /// <summary>Web service listening port</summary>
        public int Port { get; }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig { get; }

        public Config(IConfigData configData)
        {
            this.Port = configData.GetInt(PortKey);

            this.ServicesConfig = new ServicesConfig
            {
                RulesTemplatesFolder = MapRelativePath(configData.GetString(RulesTemplatesFolderKey)),
                StorageConnString = configData.GetString(StorageConnStringKey),
                KeyValueStorageApiUrl = configData.GetString(KeyValueStorageApiUrlKey),
                KeyValueStorageApiTimeout = configData.GetInt(KeyValueStorageApiTimeoutKey)
            };
        }

        private static string MapRelativePath(string path)
        {
            if (path.StartsWith(".")) return AppContext.BaseDirectory + Path.DirectorySeparatorChar + path;
            return path;
        }
    }
}
