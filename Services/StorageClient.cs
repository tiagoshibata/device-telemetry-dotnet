// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime;
using System;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services
{
    public interface IStorageClient
    {
        DocumentClient getDocumentClient();

        Tuple<bool, string> Ping();
    }
    public class StorageClient : IStorageClient
    {
        private readonly ILogger log;
        private Uri storageUri;
        private string storagePrimaryKey;
        private DocumentClient client;

        public StorageClient(
            IServicesConfig config,
            ILogger logger)
        {
            this.storageUri = config.DocumentDbUri;
            this.storagePrimaryKey = config.DocumentDbKey;
            this.log = logger;
            this.client = getDocumentClient();
        }

        public DocumentClient getDocumentClient()
        {
            if (this.client == null)
            {
                this.client = new DocumentClient(
                    this.storageUri,
                    this.storagePrimaryKey,
                    ConnectionPolicy.Default,
                    ConsistencyLevel.Session);

                if (this.client == null)
                {
                    this.log.Error("Could not connect to DocumentClient",() => new { this.storageUri });
                    throw new InvalidConfigurationException("Could not connect to DocumentClient");
                }
            }

            return this.client;
        }

        public Tuple<bool, string> Ping()
        {
            Uri response = null;

            if (this.client != null)
            {
                // make generic call to see if storage client can be reached
                response = this.client.ReadEndpoint;
            }

            if (response != null)
            {
                return new Tuple<bool, string>(true, "OK: Alive and well");
            }
            else
            {
                return new Tuple<bool, string>(false, "Could not reach storage service. " +
                    "Check connection string");
            }
        }
    }
}
