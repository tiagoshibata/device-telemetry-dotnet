// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Models;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.Runtime;
using Microsoft.Azure.IoTSolutions.DeviceTelemetry.Services.StorageAdapter;
using Moq;
using Services.Test.helpers;
using Xunit;

namespace Services.Test
{
    public class RulesTest
    {
        private readonly Mock<IStorageAdapterClient> storageAdapter;
        private readonly Mock<ILogger> logger;
        private readonly Mock<IServicesConfig> servicesConfig;
        private readonly Mock<IRules> rules;

        public RulesTest()
        {
            this.storageAdapter = new Mock<IStorageAdapterClient>();
            this.logger = new Mock<ILogger>();
            this.servicesConfig = new Mock<IServicesConfig>();
            this.rules = new Mock<IRules>();
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task InitialListIsEmptyAsync()
        {
            // Arrange
            this.ThereAreNoRulessInStorage();

            // Act
            var list = await this.rules.Object.GetListAsync(null, 0, 1000, null);

            // Assert
            Assert.Equal(0, list.Count);
        }

        [Fact, Trait(Constants.TYPE, Constants.UNIT_TEST)]
        public async Task GetListWithValuesAsync()
        {
            // Arrange
            this.ThereAreSomeRulesInStorage();

            // Act
            var list = await this.rules.Object.GetListAsync(null, 0, 1000, null);

            // Assert
            Assert.NotEmpty(list);
        }

        private void ThereAreNoRulessInStorage()
        {
            this.rules.Setup(x => x.GetListAsync(null, 0, 1000, null))
                .ReturnsAsync(new List<Rule>());
        }

        private void ThereAreSomeRulesInStorage()
        {
            var sampleConditions = new List<Condition>
            {
                new Condition("sample_conddition","Equals","1")
            };

            var sampleRules = new List<Rule>
            {
                new Rule(
                    "Sample 1",
                    true,
                    "Sample description 1",
                    "Prototyping devices",
                    "critical",
                    sampleConditions),
                new Rule(
                    "Sample 2",
                    true,
                    "Sample description 2",
                    "Prototyping devices",
                    "warning",
                    sampleConditions)
            };

            this.rules.Setup(x => x.GetListAsync(null, 0, 1000, null))
                .ReturnsAsync(sampleRules);
        }
    }
}
