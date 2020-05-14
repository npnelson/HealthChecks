using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;

namespace NetToolBox.HealthChecks.Core
{
    public sealed class HealthReportEntryResult
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
