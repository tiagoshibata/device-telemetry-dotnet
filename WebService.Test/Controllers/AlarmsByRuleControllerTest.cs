// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.Documents;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.Runtime;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.WebService.v1.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using WebService.Test.helpers;
using Xunit;
using Alarm = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Alarm;

namespace WebService.Test.Controllers
{
    class AlarmsByRuleControllerTest
    {
        private AlarmsByRuleController controller;

        private readonly Mock<ILogger> log;
        private readonly IStorageClient storage;

        private List<Alarm> sampleAlarms;

        private string docSchemaKey = "doc.schema";
        private string docSchemaValue = "alarm";

        private string docSchemaVersionKey = "doc.schemaVersion";
        private int docSchemaVersionValue = 1;

        private string createdKey = "created";
        private string modifiedKey = "modified";
        private string descriptionKey = "description";
        private string statusKey = "status";
        private string deviceIdKey = "device.id";

        private string ruleIdKey = "rule.id";
        private string ruleSeverityKey = "rule.severity";
        private string ruleDescriptionKey = "rule.description";

        public AlarmsByRuleControllerTest()
        {
            ConfigData configData = new ConfigData();
            Config config = new Config(configData);
            IServicesConfig servicesConfig = config.ServicesConfig;
            this.log = new Mock<ILogger>();

            this.storage = new StorageClient(servicesConfig, log.Object);
            string dbName = servicesConfig.AlarmsConfig.DocumentDbDatabase;
            string collName = servicesConfig.AlarmsConfig.DocumentDbCollection;
            storage.CreateCollectionIfNotExistsAsync(dbName, collName);

            this.sampleAlarms = getSampleAlarms();
            foreach (Alarm sampleAlarm in this.sampleAlarms)
            {
                storage.UpsertDocumentAsync(
                    dbName,
                    collName,
                    AlarmToDocument(sampleAlarm));
            }

            Alarms alarmService = new Alarms(servicesConfig, this.storage, this.log.Object);
            this.controller = new AlarmsByRuleController(alarmService, this.log.Object);
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public void ProvideAlarmsByRuleResult()
        {
            // Act
            var response = this.controller.List(null, null, "asc", null, null, null);

            // Assert
            Assert.NotEmpty(response.Metadata);
            Assert.NotEmpty(response.Items);
        }

        private Document AlarmToDocument(Alarm alarm)
        {
            Document document = new Document()
            {
                Id = Guid.NewGuid().ToString()
            };

            document.SetPropertyValue(docSchemaKey, docSchemaValue);
            document.SetPropertyValue(docSchemaVersionKey, docSchemaVersionValue);
            document.SetPropertyValue(createdKey, alarm.DateCreated.ToUnixTimeMilliseconds());
            document.SetPropertyValue(modifiedKey, alarm.DateModified.ToUnixTimeMilliseconds());
            document.SetPropertyValue(statusKey, alarm.Status);
            document.SetPropertyValue(descriptionKey, alarm.Description);
            document.SetPropertyValue(deviceIdKey, alarm.DeviceId);
            document.SetPropertyValue(ruleIdKey, alarm.RuleId);
            document.SetPropertyValue(ruleSeverityKey, alarm.RuleSeverity);
            document.SetPropertyValue(ruleDescriptionKey, alarm.RuleDescription);

            // The logic used to generate the alarm (future proofing for ML)
            document.SetPropertyValue("logic", "1Device-1Rule-1Message");

            return document;
        }

        private List<Alarm> getSampleAlarms()
        {
            List<Alarm> list = new List<Alarm>();

            Alarm alarm1 = new Alarm(
                null,
                "1",
                DateTimeOffset.Parse("2017-07-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                DateTimeOffset.Parse("2017-07-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                "Temperature on device x > 75 deg F",
                "group-Id",
                "device-id",
                "open",
                "1",
                "critical",
                "HVAC temp > 50"
            );

            Alarm alarm2 = new Alarm(
                null,
                "2",
                DateTimeOffset.Parse("2017-06-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                DateTimeOffset.Parse("2017-07-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                "Temperature on device x > 75 deg F",
                "group-Id",
                "device-id",
                "acknowledged",
                "2",
                "critical",
                "HVAC temp > 60");

            Alarm alarm3 = new Alarm(
                null,
                "3",
                DateTimeOffset.Parse("2017-05-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                DateTimeOffset.Parse("2017-06-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                "Temperature on device x > 75 deg F",
                "group-Id",
                "device-id",
                "open",
                "3",
                "info",
                "HVAC temp > 70");

            Alarm alarm4 = new Alarm(
                null,
                "4",
                DateTimeOffset.Parse("2017-04-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                DateTimeOffset.Parse("2017-06-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                "Temperature on device x > 75 deg F",
                "group-Id",
                "device-id",
                "closed",
                "4",
                "warning",
                "HVAC temp > 80");

            list.Add(alarm1);
            list.Add(alarm2);
            list.Add(alarm3);
            list.Add(alarm4);

            return list;
        }

        private Alarm getSampleAlarm()
        {
            return new Alarm(
                "6l1log0f7h2yt6p",
                "1234",
                DateTimeOffset.Parse("2017-02-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                DateTimeOffset.Parse("2017-02-22T22:22:22-08:00").ToUnixTimeMilliseconds(),
                "Temperature on device x > 75 deg F",
                "group-Id",
                "device-id",
                "open",
                "1234",
                "critical",
                "HVAC temp > 75"
            );
        }
    }
}
