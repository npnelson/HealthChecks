using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;

namespace NetToolBox.HealthChecks.Functions
{
    internal sealed class HealthReportResult
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
        public string Status;
        //public string ApplicationVersion => _version;
        //public string CLRVersion { get; set; }
        //public string OSVersion { get; set; }
        public TimeSpan TotalDuration;
        public IReadOnlyDictionary<string, HealthReportEntryResult> Results;

    }

    public static class HealthReportObjectResult
    {
        public static ObjectResult GetHealthReportObjectResult(HealthReport report)
        {
            var reportResult = new HealthReportResult(report);
            var result = new ObjectResult(reportResult);
            if (report.Status == HealthStatus.Degraded) result.StatusCode = 598;
            if (report.Status == HealthStatus.Unhealthy) result.StatusCode = 599;
            return result;
        }
    }

    internal sealed class HealthReportEntryResult
    {


        public HealthReportEntryResult(HealthReportEntry entry)
        {

            Data = entry.Data;
            Description = entry.Description;
            Duration = entry.Duration;
            Exception = entry.Exception;
            Tags = entry.Tags;
            Status = HealthReportResult.GetHealthStatusString(entry.Status);
        }

        public readonly IReadOnlyDictionary<string, object> Data;
        //
        // Summary:
        //     Gets a human-readable description of the status of the component that was checked.
        public readonly string Description;
        //
        // Summary:
        //     Gets the health check execution duration.
        public readonly TimeSpan Duration;
        //
        // Summary:
        //     Gets an System.Exception representing the exception that was thrown when checking
        //     for status (if any).
        public readonly Exception Exception;
        //
        // Summary:
        //     Gets the health status of the component that was checked.
        public readonly string Status;
        //
        // Summary:
        //     Gets the tags associated with the health check.
        public readonly IEnumerable<string> Tags;

    }
}
