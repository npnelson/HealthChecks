using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetToolBox.DateTimeService;
using NETToolBox.BlobStorage.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static NetToolBox.HealthChecks.AzureFunctionTimer.NcronTabExtensions;

namespace NetToolBox.HealthChecks.AzureFunctionTimer
{
    internal sealed class TimerTriggerHealthCheck : IHealthCheck
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IBlobStorage _blobStorage;
        private readonly TimerTriggerHealthCheckOptions _options;
        private readonly List<TimerTriggerInfo> _timerTriggers;
        private readonly TimerHealthCheckHelperOptions _helperOptions;

        public TimerTriggerHealthCheck(IBlobStorageFactory blobFactory, IDateTimeService dateTimeService, TimerTriggerHealthCheckOptions options, TimerHealthCheckHelperOptions helperOptions, List<TimerTriggerInfo> timerTriggers)
        {
            _dateTimeService = dateTimeService;
            _blobStorage = blobFactory.GetBlobStorage(helperOptions.AzureWebJobsStorageConnectionString, "azure-webjobs-hosts");
            _options = options;
            _timerTriggers = timerTriggers;
            _helperOptions = helperOptions;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var itemDictionary = new Dictionary<string, object>();
            HealthCheckResult retval;

            try
            {
                foreach (var trigger in _timerTriggers)
                {

                    //get last execution completion time
                    var blobPath = TimerHealthCheckHelper.GetBlobPath(trigger.TimerFullTypeName, _helperOptions);
                    var lastTimeString = await _blobStorage.DownloadFileAsTextAsync(blobPath).ConfigureAwait(false);
                    TimerHealthCheckStatus status = JsonSerializer.Deserialize<TimerHealthCheckStatus>(lastTimeString);
                    var lastExpectedTime = GetLastScheduledOccurrence(trigger.ScheduleExpression, _dateTimeService.CurrentDateTimeOffset.DateTime);
                    itemDictionary.Add(trigger.TimerFullTypeName, new TimerTriggerHealthResult { LastCompletionTime = status.LastCheckpoint, LastExpectedCompletionTime = new DateTimeOffset(lastExpectedTime, status.LastCheckpoint.Offset) });
                }
                if (itemDictionary.Values.Any(x => (((TimerTriggerHealthResult)x).LastCompletionTime + _options.ToleranceTimeSpan) < ((TimerTriggerHealthResult)x).LastExpectedCompletionTime))
                {
                    retval = new HealthCheckResult(HealthStatus.Unhealthy, "Some timers have not fired on time", null, itemDictionary);

                }
                else
                {
                    retval = new HealthCheckResult(HealthStatus.Healthy, null, null, itemDictionary);
                }

            }
            catch (Exception ex)
            {

                retval = new HealthCheckResult(HealthStatus.Unhealthy, "", ex, itemDictionary);
            }

            return retval;
        }
    }
}
