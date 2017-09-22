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

        Task<Alarm> UpdateAsync(string id, string status);
    }

    public class Alarms : IAlarms
    {
        private const string INVALID_CHARACTER = @"[^A-Za-z0-9:;.,_\-]";

        private readonly ILogger log;
        private readonly IStorageClient storageClient;

        private readonly string databaseName;
        private readonly string collectionId;

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
            string sql = QueryBuilder.BuildSql(
                "alarm",
                null, null,
                from, "created",
                to, "created",
                order, "created",
                skip,
                limit,
                devices, "device.id");

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
            string sql = QueryBuilder.BuildSql(
                "alarm",
                id, "rule.id",
                from, "created",
                to, "created",
                order, "created",
                skip,
                limit,
                devices, "device.id");

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

        public async Task<Alarm> UpdateAsync(string id, string status)
        {
            if (Regex.IsMatch(id, INVALID_CHARACTER))
            {
                throw new InvalidInputException("id contains illegal characters.");
            }

            Document document = this.GetDocumentById(id);
            document.SetPropertyValue("status", status);

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
                1000);

            if (documentList.Count > 0)
            {
                return documentList[0];
            }
            else
            {
                return null;
            }
        }
    }
}
