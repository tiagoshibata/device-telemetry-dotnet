// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Helpers;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Message = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Message;
using MessageList = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.MessageList;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services
{
    public interface IMessages
    {
        MessageList GetList(
            DateTimeOffset? from,
            DateTimeOffset? to,
            string order,
            int skip,
            int limit,
            string[] devices);
    }
    public class Messages : IMessages
    {
        private readonly ILogger log;
        private readonly IStorageClient storageClient;

        private readonly string dataPrefix = "data.";

        private readonly DocumentClient docDbConnection;
        private readonly string docDbCollectionLink;

        public Messages(
            IServicesConfig config,
            IStorageClient storageClient,
            ILogger logger)
        {
            this.storageClient = storageClient;
            this.docDbConnection = storageClient.getDocumentClient();
            this.docDbCollectionLink = string.Format(
                "/dbs/{0}/colls/{1}",
                config.MessagesConfig.DocumentDbDatabase,
                config.MessagesConfig.DocumentDbCollection);
            this.log = logger;
        }

        public MessageList GetList(
            DateTimeOffset? from,
            DateTimeOffset? to,
            String order,
            int skip,
            int limit,
            String[] devices)
        {

            int dataPrefixLen = dataPrefix.Length;

            String sql = QueryBuilder.buildSQL(
                "d2cmessage",
                null, null,
                from, "device.msg.received",
                to, "device.msg.received",
                order, "device.msg.received",
                skip,
                limit,
                devices, "device.id");

            this.log.Debug("Created Message Query", () => new { sql });

            List<Document> docs = query(sql, skip);

            // Messages to return
            List<Message> messages = new List<Message>();

            // Auto discovered telemetry types
            HashSet<string> properties = new HashSet<string>();

            foreach (Document doc in docs)
            {
                // Document fields to expose
                List<KeyValuePair<string, object>> data = new List<KeyValuePair<string, object>>();

                // Extract all the telemetry data and types
                var jsonDoc = JObject.Parse(doc.ToString());
                foreach (var item in jsonDoc)
                {
                    // Ignore fields that don't start with "data."
                    if (item.Key.ToString().StartsWith(dataPrefix))
                    {
                        // Remove the "data." prefix
                        string key = item.Key.ToString().Substring(dataPrefixLen);
                        data.Add(new KeyValuePair<string, object>(key, item.Value));

                        // Telemetry types auto-discovery magic
                        properties.Add(key);
                    }
                }

                messages.Add(new Message(
                    doc.GetPropertyValue<string>("device.id"),
                    doc.GetPropertyValue<long>("device.msg.received"),
                    data));
            }

            return new MessageList(messages, new List<string>(properties));
        }

        private List<Document> query(string sql, int skip)
        {
            FeedOptions queryOptions = new FeedOptions();
            queryOptions.EnableCrossPartitionQuery = true;
            queryOptions.EnableScanInQuery = true;

            List<Document> docs = new List<Document>();
            String continuationToken = null;

            SqlQuerySpec sqlTest = new SqlQuerySpec(sql);

            do
            {
                var query = this.docDbConnection.CreateDocumentQuery<Document>(
                    this.docDbCollectionLink,
                    sqlTest,
                    queryOptions).AsDocumentQuery();

                FeedResponse<Document> queryResults = query.ExecuteNextAsync<Document>().Result;

                foreach (Document doc in queryResults)
                {
                    if (skip == 0)
                    {
                        docs.Add(doc);
                    }
                    else
                    {
                        skip--;
                    }
                }

                continuationToken = queryResults.ResponseContinuation;
                queryOptions.RequestContinuation = continuationToken;
            } while (!String.IsNullOrEmpty(continuationToken));

            this.log.Debug("Message query results count:", () => new { docs.Count });

            return docs;
        }
    }
}
