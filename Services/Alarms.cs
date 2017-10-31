// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Helpers;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services
{
    public interface IAlarms
    {
        Alarm Get(string id);

        List<Alarm> List(
            DateTimeOffset? from,
            DateTimeOffset? to,
            string order,
            int skip,
            int limit,
            string[] devices);

        List<Alarm> ListByRule(
            string id,
            DateTimeOffset? from,
            DateTimeOffset? to,
            string order,
            int skip,
            int limit,
            string[] devices);

        int GetCountByRule(
            string id,
            DateTimeOffset? from,
            DateTimeOffset? to,
            string[] devices);

            Task<Alarm> UpdateAsync(string id, string status);
    }

    public class Alarms : IAlarms
    {
        private const string INVALID_CHARACTER = @"[^A-Za-z0-9:;.,_\-]";

        private readonly ILogger log;
        private readonly IStorageClient storageClient;

        private readonly string databaseName;
        private readonly string collectionId;

        // constants for storage keys
        private const string MESSAGE_RECEIVED_KEY = "device.msg.received";
        private const string RULE_ID_KEY = "rule.id";
        private const string DEVICE_ID_KEY = "device.id";
        private const string STATUS_KEY = "status";
        private const string ALARM_SCHEMA_KEY = "alarm";

        private const string ALARM_STATUS_OPEN = "open";
        private const string ALARM_STATUS_ACKNOWLEDGED = "acknowledged";

        private const int DOC_QUERY_LIMIT = 1000;

        public Alarms(
            IServicesConfig config,
            IStorageClient storageClient,
            ILogger logger)
        {
            this.storageClient = storageClient;
            this.databaseName = config.AlarmsConfig.DocumentDbDatabase;
            this.collectionId = config.AlarmsConfig.DocumentDbCollection;
            this.log = logger;
        }

        public Alarm Get(string id)
        {
            if (Regex.IsMatch(id, INVALID_CHARACTER))
            {
                throw new InvalidInputException("id contains illegal characters.");
            }

            Document doc = this.GetDocumentById(id);
            return new Alarm(doc);
        }

        public List<Alarm> List(
            DateTimeOffset? from,
            DateTimeOffset? to,
            string order,
            int skip,
            int limit,
            string[] devices)
        {
            string sql = QueryBuilder.GetDocumentsSql(
                ALARM_SCHEMA_KEY,
                null, null,
                from, MESSAGE_RECEIVED_KEY,
                to, MESSAGE_RECEIVED_KEY,
                order, MESSAGE_RECEIVED_KEY,
                skip,
                limit,
                devices, DEVICE_ID_KEY);

            this.log.Debug("Created Alarm Query", () => new { sql });

            FeedOptions queryOptions = new FeedOptions();
            queryOptions.EnableCrossPartitionQuery = true;
            queryOptions.EnableScanInQuery = true;

            List<Document> docs = this.storageClient.QueryDocuments(
                this.databaseName,
                this.collectionId,
                queryOptions,
                sql,
                skip,
                limit);

            List<Alarm> alarms = new List<Alarm>();

            foreach (Document doc in docs)
            {
                alarms.Add(new Alarm(doc));
            }

            return alarms;
        }

        public List<Alarm> ListByRule(
            string id,
            DateTimeOffset? from,
            DateTimeOffset? to,
            string order,
            int skip,
            int limit,
            string[] devices)
        {
            string sql = QueryBuilder.GetDocumentsSql(
                ALARM_SCHEMA_KEY,
                id, RULE_ID_KEY,
                from, MESSAGE_RECEIVED_KEY,
                to, MESSAGE_RECEIVED_KEY,
                order, MESSAGE_RECEIVED_KEY,
                skip,
                limit,
                devices, DEVICE_ID_KEY);

            if (Regex.IsMatch(id, INVALID_CHARACTER))
            {
                throw new InvalidInputException("id contains illegal characters.");
            }

            this.log.Debug("Created Alarm By Rule Query", () => new { sql });

            FeedOptions queryOptions = new FeedOptions();
            queryOptions.EnableCrossPartitionQuery = true;
            queryOptions.EnableScanInQuery = true;

            List<Document> docs = this.storageClient.QueryDocuments(
                this.databaseName,
                this.collectionId,
                queryOptions,
                sql,
                skip,
                limit);

            List<Alarm> alarms = new List<Alarm>();
            foreach (Document doc in docs)
            {
                alarms.Add(new Alarm(doc));
            }

            return alarms;
        }

        public int GetCountByRule(
            string id,
            DateTimeOffset? from,
            DateTimeOffset? to,
            string[] devices)
        {
            // build sql query to get open/acknowledged alarm count for rule
            string[] statusList = { ALARM_STATUS_OPEN, ALARM_STATUS_ACKNOWLEDGED };
            string sql = QueryBuilder.GetCountSql(
                ALARM_SCHEMA_KEY,
                id, RULE_ID_KEY,
                from, MESSAGE_RECEIVED_KEY,
                to, MESSAGE_RECEIVED_KEY,
                devices, DEVICE_ID_KEY,
                statusList, STATUS_KEY);

            FeedOptions queryOptions = new FeedOptions();
            queryOptions.EnableCrossPartitionQuery = true;
            queryOptions.EnableScanInQuery = true;

            // request count of alarms for a rule id with given parameters
            var result = this.storageClient.QueryCount(
                this.databaseName,
                this.collectionId,
                queryOptions,
                sql);

            return result;
        }

        public async Task<Alarm> UpdateAsync(string id, string status)
        {
            if (Regex.IsMatch(id, INVALID_CHARACTER))
            {
                throw new InvalidInputException("id contains illegal characters.");
            }

            Document document = this.GetDocumentById(id);
            document.SetPropertyValue(STATUS_KEY, status);

            document = await this.storageClient.UpsertDocumentAsync(
                this.databaseName,
                this.collectionId,
                document);

            return new Alarm(document);
        }

        private Document GetDocumentById(string id)
        {
            if (Regex.IsMatch(id, INVALID_CHARACTER))
            {
                throw new InvalidInputException("id contains illegal characters.");
            }

            // Retrieve the document using the DocumentClient.
            List<Document> documentList = this.storageClient.QueryDocuments(
                this.databaseName,
                this.collectionId,
                null,
                "SELECT * FROM c WHERE c.id='" + id + "'",
                0,
                DOC_QUERY_LIMIT);

            if (documentList.Count > 0)
            {
                return documentList[0];
            }

            return null;
        }
    }
}
