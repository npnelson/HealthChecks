using Microsoft.Extensions.Diagnostics.HealthChecks;
using NETToolBox.BlobStorage.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetToolBox.HealthChecks.BlobStorage
{
    internal sealed class BlobStorageHealthCheck : IHealthCheck
    {
        private readonly IBlobStorageFactory _blobFactory;

        public BlobStorageHealthCheck(IBlobStorageFactory blobFactory)
        {
            _blobFactory = blobFactory;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var itemDictionary = new Dictionary<string, object>();

            bool isHealthy = true;
            foreach (var container in _blobFactory.GetBlobStorageRegistrations())
            {
                try
                {
                    var result = await _blobFactory.GetBlobStorage(container.accountName, container.containerName).IsHealthyAsync().ConfigureAwait(false);
                    if (result)
                    {
                        itemDictionary.Add($"{container.accountName}:{container.containerName}", "OK");
                    }
                    else
                    {
                        itemDictionary.Add($"{container.accountName}:{container.containerName}", "HealthCheck returned unhealthy"); //this will probably never get a false, would more likely throw exception
                        isHealthy = false;
                    }
                }
                catch (Exception ex)
                {
                    isHealthy = false;
                    itemDictionary.Add($"{container.accountName}:{container.containerName}", ex.ToString());
                }
            }
            return new HealthCheckResult(isHealthy ? HealthStatus.Healthy : HealthStatus.Unhealthy, null, null, itemDictionary);

        }
    }
}
