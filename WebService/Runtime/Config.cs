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
        private const string APPLICATION_KEY = "telemetry:";
        private const string PORT_KEY = APPLICATION_KEY + "webservice_port";

        private const string RULES_TEMPLATES_FOLDER_KEY = APPLICATION_KEY + "rules_templates_folder";

        private const string STORAGE_TYPE_KEY = APPLICATION_KEY + "storage_type";

        private const string DOCUMENTDB_KEY = "documentdb:";
        private const string DOCUMENTDB_CONNSTRING_KEY = DOCUMENTDB_KEY + "connstring";
        private const string DOCUMENTDB_DATABASE_KEY = DOCUMENTDB_KEY + "database";
        private const string DOCUMENTDB_COLLECTION_KEY = DOCUMENTDB_KEY + "collection";
        private const string DOCUMENTDB_RUS_KEY = DOCUMENTDB_KEY + "RUs";

        private const string MESSAGES_DB_KEY = "messages:";
        private const string MESSAGES_DB_DATABASE_KEY = MESSAGES_DB_KEY + "database";
        private const string MESSAGES_DB_COLLECTION_KEY = MESSAGES_DB_KEY + "collection";

        private const string ALARMS_DB_KEY = "alarms:";
        private const string ALARMS_DB_DATABASE_KEY = ALARMS_DB_KEY + "database";
        private const string ALARMS_DB_COLLECTION_KEY = ALARMS_DB_KEY + "collection";

        private const string STORAGE_ADAPTER_KEY = "storageadapter:";
        private const string STORAGE_ADAPTER_API_URL_KEY = STORAGE_ADAPTER_KEY + "webservice_url";
        private const string STORAGE_ADAPTER_API_TIMEOUT_KEY = STORAGE_ADAPTER_KEY + "webservice_timeout";

        /// <summary>Web service listening port</summary>
        public int Port { get; }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig { get; }

        public Config(IConfigData configData)
        {
            this.Port = configData.GetInt(PORT_KEY);

            this.ServicesConfig = new ServicesConfig
            {
                MessagesConfig = new StorageConfig(
                    configData.GetString(MESSAGES_DB_DATABASE_KEY),
                    configData.GetString(MESSAGES_DB_COLLECTION_KEY)),
                AlarmsConfig = new StorageConfig(
                    configData.GetString(ALARMS_DB_DATABASE_KEY),
                    configData.GetString(ALARMS_DB_COLLECTION_KEY)),
                RulesTemplatesFolder = MapRelativePath(configData.GetString(RULES_TEMPLATES_FOLDER_KEY)),
                DocumentDbConnString = configData.GetString(DOCUMENTDB_CONNSTRING_KEY),
                DocumentDbThroughput = configData.GetInt(DOCUMENTDB_RUS_KEY),
                StorageAdapterApiUrl = configData.GetString(STORAGE_ADAPTER_API_URL_KEY),
                StorageAdapterApiTimeout = configData.GetInt(STORAGE_ADAPTER_API_TIMEOUT_KEY)
            };
        }

        private static string MapRelativePath(string path)
        {
            if (path.StartsWith(".")) return AppContext.BaseDirectory + Path.DirectorySeparatorChar + path;
            return path;
        }
    }
}
