using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetToolBox.Search.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetToolBox.HealthChecks.Search
{
    internal sealed class SearchHealthCheck : IHealthCheck
    {
        private readonly ISearchClient _searchClient;

        public SearchHealthCheck(ISearchClient searchClient)
        {
            _searchClient = searchClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var indexes = await _searchClient.ListIndexesAsync().ConfigureAwait(false);
                var indexNames = "Indexes Available:" + string.Join(",", indexes);
                return new HealthCheckResult(HealthStatus.Healthy, indexNames, null, null);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "ListIndexesAsync threw Exception", ex, null);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}