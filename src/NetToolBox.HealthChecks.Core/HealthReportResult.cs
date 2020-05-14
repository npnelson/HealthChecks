using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;

namespace NetToolBox.HealthChecks.Core
{
    public sealed class HealthReportResult
    {

        internal static string GetHealthStatusString(HealthStatus status) =>
            status switch
            {
                HealthStatus.Degraded => "Degraded",
                HealthStatus.Healthy => "Healthy",
                HealthStatus.Unhealthy => "Unhealthy",
                _ => throw new ArgumentException("Unknown healthstatus value", paramName: nameof(status)),
            };
        internal HealthReportResult(HealthReport healthReport)
        {
            Status = GetHealthStatusString(healthReport.Status);
            TotalDuration = healthReport.TotalDuration;
            var resultDictionary = new Dictionary<string, HealthReportEntryResult>();
            foreach (var result in healthReport.Entries)
            {
                resultDictionary.Add(result.Key, new HealthReportEntryResult(result.Value));
            }
            Results = resultDictionary;
        }
        public readonly string Status;
        public readonly TimeSpan TotalDuration;
        public readonly IReadOnlyDictionary<string, HealthReportEntryResult> Results;

    }
}
