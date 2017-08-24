// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime;
using Moq;
using Newtonsoft.Json.Linq;
using Services.Test.helpers;
using System;
using System.Collections.Generic;
using Xunit;
using Message = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Message;
using MessageList = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.MessageList;

namespace Services.Test
{
    public class MessagesTest
    {
        private const int skip = 0;
        private const int limit = 1000;

        private readonly Mock<IStorageClient> storage;
        private readonly Mock<ILogger> logger;
        private readonly Mock<IServicesConfig> servicesConfig;
        private readonly Mock<IMessages> messages;

        private readonly DocumentClient documentClient;

        private readonly Messages target;

        private readonly List<Message> messageList;

        public MessagesTest()
        {
            this.storage = new Mock<IStorageClient>();
            this.logger = new Mock<ILogger>();
            this.servicesConfig = new Mock<IServicesConfig>();
            this.messages = new Mock<IMessages>();
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public void InitialListIsEmpty()
        {
            // Arrange
            this.ThereAreNoMessagesInStorage();

            // Act
            var list = this.messages.Object.GetList(null, null, null, skip, limit, null);
            var messageList = list.Messages;
            var propertyList = list.Properties;

            // Assert
            Assert.Equal(0, messageList.Count);
            Assert.Equal(0, propertyList.Count);
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public void GetListWithValues()
        {
            // Arrange
            this.ThereAreSomeMessagesInStorage();

            // Act
            var list = this.messages.Object.GetList(null, null, null, skip, limit, null);
            var messageList = list.Messages;
            var propertyList = list.Properties;

            // Assert
            Assert.NotEmpty(messageList);
            Assert.NotEmpty(propertyList);
        }

        private void ThereAreNoMessagesInStorage()
        {
            this.messages.Setup(x => x.GetList(null, null, null, skip, limit, null))
                .Returns(new MessageList());
        }

        private void ThereAreSomeMessagesInStorage()
        {
            this.storage.Setup(x => x.getDocumentClient())
                .Returns(this.storage.Object.getDocumentClient());

            List<Message> sampleMessages = new List<Message>();
            List<string> sampleProperties = new List<string>();
            JObject data = new JObject();

            data.Add("data.sample_unit", "mph");
            data.Add("data.sample_speed", "10");

            sampleMessages.Add(new Message("id1", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), data));
            sampleMessages.Add(new Message("id2", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), data));

            sampleProperties.Add("data.sample_unit");
            sampleProperties.Add("data.sample_speed");

            this.messages.Setup(x => x.GetList(null, null, null, skip, limit, null))
                .Returns(new MessageList(sampleMessages, sampleProperties));
        }
    }
}
