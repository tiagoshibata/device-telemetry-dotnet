// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services;
using Moq;
using Newtonsoft.Json.Linq;
using Services.Test.helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Message = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.Message;
using MessageList = Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models.MessageList;

namespace Services.Test
{
    public class MessagesTest
    {
        private const int skip = 0;
        private const int limit = 1000;

        private readonly Mock<IMessages> messages;

        public MessagesTest()
        {
            this.messages = new Mock<IMessages>();
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public void InitialListIsEmpty()
        {
            // Arrange
            this.ThereAreNoMessagesInStorage();

            // Act
            var list = this.messages.Object.List(null, null, null, skip, limit, null);

            // Assert
            Assert.Equal(0, list.Messages.Count);
            Assert.Equal(0, list.Properties.Count);
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public void GetListWithValues()
        {
            // Arrange
            this.ThereAreSomeMessagesInStorage();

            // Act
            var list = this.messages.Object.List(null, null, null, skip, limit, null);

            // Assert
            Assert.NotEmpty(list.Messages);
            Assert.NotEmpty(list.Properties);
        }

        private void ThereAreNoMessagesInStorage()
        {
            this.messages.Setup(x => x.List(null, null, null, skip, limit, null))
                .Returns(new MessageList());
        }

        private void ThereAreSomeMessagesInStorage()
        {
            List<Message> sampleMessages = new List<Message>();
            List<string> sampleProperties = new List<string>();

            JObject data = new JObject
            {
                { "data.sample_unit", "mph" },
                { "data.sample_speed", "10" }
            };

            sampleMessages.Add(new Message("id1", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), data));
            sampleMessages.Add(new Message("id2", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), data));

            sampleProperties.Add("data.sample_unit");
            sampleProperties.Add("data.sample_speed");

            this.messages.Setup(x => x.List(null, null, null, skip, limit, null))
                .Returns(new MessageList(sampleMessages, sampleProperties));
        }
    }
}
