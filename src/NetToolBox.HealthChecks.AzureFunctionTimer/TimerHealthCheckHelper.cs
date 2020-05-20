using Microsoft.Extensions.Logging;
using NetToolBox.DateTimeService;
using NETToolBox.BlobStorage.Abstractions;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetToolBox.HealthChecks.AzureFunctionTimer
{
    public class TimerHealthCheckHelper
    {
        private readonly IBlobStorageFactory _blobStorageFactory;
        private readonly TimerHealthCheckHelperOptions _options;
        private readonly IDateTimeService _dateTimeService;
        private readonly ILogger<TimerHealthCheckHelper> _logger;

        public TimerHealthCheckHelper(IBlobStorageFactory blobStorageFactory, IDateTimeService dateTimeService, TimerHealthCheckHelperOptions options, ILogger<TimerHealthCheckHelper> logger)
        {
            _blobStorageFactory = blobStorageFactory;
            _options = options;
            _dateTimeService = dateTimeService;
            _logger = logger;
        }
        /// <summary>
        /// Call this method at the end of your timer so it can checkpoint successful completion
        /// </summary>
        /// <typeparam name="T">The Type of your timertrigger</typeparam>
        /// <returns></returns>
        public async Task CheckpointMethod<T>()
        {

            var blobContainer = _blobStorageFactory.GetBlobStorage(_options.AzureWebJobsStorageConnectionString, "azure-webjobs-hosts");
            var status = new TimerHealthCheckStatus { LastCheckpoint = _dateTimeService.CurrentDateTimeOffset };
            if (_options.IsProductionSlot)
            {
                var blobPath = GetBlobPath(typeof(T).FullName, _options);
                await blobContainer.StoreBlobAsTextAsync(blobPath, JsonSerializer.Serialize(status)).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(_options.AzureWebSiteName))
                {
                    _logger.LogWarning("It appears you are running locally. Timer will be checkpointed, but if you are not running locally, make sure you include the APPSETTING_WEBSITE_SITE_NAME to make sure this timer's checkpoint is unique to this instance");
                }
            }
            else
            {
                _logger.LogWarning("Timer is not running in a production slot, timer will not be checkpointed");
            }
        }
        internal static string GetBlobPath(string? fullTypeName, TimerHealthCheckHelperOptions options)
        {
            if (fullTypeName == null) throw new ArgumentNullException(nameof(fullTypeName), "Full Type Name must not be null. This exception should never happen");
            if (!string.IsNullOrWhiteSpace(options.AzureWebSiteName))
            {
                return $"timercheckpoints/{options.AzureWebSiteName}/{fullTypeName}/status";
            }
            else
            {
                return $"timercheckpoints/{fullTypeName}/status";
            }
        }
    }
}
