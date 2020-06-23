using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NetToolBox.DateTimeService.TestHelper;
using NetToolBox.HealthChecks.AzureFunctionTimer;
using NETToolBox.BlobStorage.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NetToolBox.HealthChecks.Tests
{
    public sealed class TimerHealthCheckTests
    {
        [Fact]
        public async Task WithinToleranceTest()
        {
            var fixture = new TimerHealthCheckTestFixture();
            fixture.TestDateTimeServiceProvider.SetCurrentDateTimeUTC(new DateTime(2020, 6, 23, 9, 5, 0, 0));
            fixture.SetLastCompletedTime(new DateTime(2020, 6, 23, 8, 0, 0, 0));

            var result = await fixture.HealthCheck.CheckHealthAsync(new HealthCheckContext());
            result.Status.Should().Be(HealthStatus.Healthy);
        }
        [Fact]
        public async Task OutsideToleranceTest()
        {
            var fixture = new TimerHealthCheckTestFixture();
            fixture.TestDateTimeServiceProvider.SetCurrentDateTimeUTC(new DateTime(2020, 11, 23, 9, 5, 1, 0));
            fixture.SetLastCompletedTime(new DateTime(2020, 11, 23, 8, 0, 0, 0));

            var result = await fixture.HealthCheck.CheckHealthAsync(new HealthCheckContext());
            result.Status.Should().Be(HealthStatus.Unhealthy);
            result.Description.Should().Be("Timer TimerFriendlyName did not fire on time - LastCompletedTime = 11/23/2020 08:00:00 -05:00 Last Expected Time = 11/23/2020 09:00:00 -05:00\n");
        }

        internal sealed class TimerHealthCheckTestFixture
        {
            internal TimerTriggerHealthCheck HealthCheck;
            public TestDateTimeServiceProvider TestDateTimeServiceProvider = new TestDateTimeServiceProvider();
            public Mock<IBlobStorageFactory> MockBlobStorageFactory = new Mock<IBlobStorageFactory>();
            public List<TimerTriggerInfo> TimerTriggerInfos = new List<TimerTriggerInfo> { new TimerTriggerInfo { IsTimerDisabled = false, TimerFullTypeName = "TestTimerFullName", TimerTriggerFriendlyName = "TimerFriendlyName", ScheduleExpression = "0 0 * * * *" } };
            public TimerHealthCheckHelperOptions HelperOptions = new TimerHealthCheckHelperOptions { IsProductionSlot = true, AzureWebSiteName = "TestWebSite", AzureWebJobsStorageConnectionString = "ConnectionString" };

            private readonly Mock<IBlobStorage> MockBlobStorage = new Mock<IBlobStorage>();
            public void SetLastCompletedTime(DateTime dateTime)
            {
                var blobPath = TimerHealthCheckHelper.GetBlobPath(TimerTriggerInfos.Single().TimerFullTypeName, HelperOptions);
                var status = new TimerHealthCheckStatus { LastCheckpoint = dateTime };
                MockBlobStorage.Setup(x => x.DownloadFileAsTextAsync(It.Is<string>(y => y == blobPath), It.IsAny<CancellationToken>())).ReturnsAsync(JsonSerializer.Serialize(status));

            }
            public TimerHealthCheckTestFixture()
            {
                MockBlobStorageFactory.Setup(x => x.GetBlobStorage(It.Is<string>(y => y == HelperOptions.AzureWebJobsStorageConnectionString), It.Is<string>(y => y == "azure-webjobs-hosts"), It.IsAny<bool>())).Returns(MockBlobStorage.Object);
                HealthCheck = new TimerTriggerHealthCheck(MockBlobStorageFactory.Object, TestDateTimeServiceProvider, new TimerTriggerHealthCheckOptions { ToleranceTimeSpan = TimeSpan.FromMinutes(5) }, HelperOptions, TimerTriggerInfos);


            }
        }
    }
}
