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

        private const string StorageTypeKey = ApplicationKey + "storage_type";

        private const string DocumentDbKey = "documentdb:";
        private const string DocumentDbConnStringKey = DocumentDbKey + "connstring";
        private const string DocumentDbDatabaseKey = DocumentDbKey + "database";
        private const string DocumentDbCollectionKey = DocumentDbKey + "collection";
        private const string DocumentDbRUsKey = DocumentDbKey + "RUs";

        private const string MessagesDbKey = "messages:";
        private const string MessagesDbDatabaseKey = MessagesDbKey + "database";
        private const string MessagesDbCollectionKey = MessagesDbKey + "collection";

        private const string AlarmsDbKey = "alarms:";
        private const string AlarmsDbDatabaseKey = AlarmsDbKey + "database";
        private const string AlarmsDbCollectionKey = AlarmsDbKey + "collection";

        private const string StorageAdapterKey = "storageadapter:";
        private const string StorageAdapterApiUrlKey = StorageAdapterKey + "webservice_url";
        private const string StorageAdapterApiTimeoutKey = StorageAdapterKey + "webservice_timeout";

        /// <summary>Web service listening port</summary>
        public int Port { get; }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig { get; }

        public Config(IConfigData configData)
        {
            this.Port = configData.GetInt(PortKey);

            this.ServicesConfig = new ServicesConfig
            {
                MessagesConfig = new StorageConfig(
                    configData.GetString(MessagesDbDatabaseKey),
                    configData.GetString(MessagesDbCollectionKey)),
                AlarmsConfig = new StorageConfig(
                    configData.GetString(AlarmsDbDatabaseKey),
                    configData.GetString(AlarmsDbCollectionKey)),
                RulesTemplatesFolder = MapRelativePath(configData.GetString(RulesTemplatesFolderKey)),
                DocumentDbConnString = configData.GetString(DocumentDbConnStringKey),
                DocumentDbThroughput = configData.GetInt(DocumentDbRUsKey),
                StorageAdapterApiUrl = configData.GetString(StorageAdapterApiUrlKey),
                StorageAdapterApiTimeout = configData.GetInt(StorageAdapterApiTimeoutKey)
            };
        }

        private static string MapRelativePath(string path)
        {
            if (path.StartsWith(".")) return AppContext.BaseDirectory + Path.DirectorySeparatorChar + path;
            return path;
        }
    }
}
