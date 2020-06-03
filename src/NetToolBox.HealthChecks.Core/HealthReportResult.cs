using Microsoft.Extensions.Diagnostics.HealthChecks;
using NETToolBox.LinuxVersion;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NetToolBox.HealthChecks.Core
{
    public sealed class HealthReportResult<T>
    {
        private static readonly string _version = typeof(T).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
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
            ApplicationVersion = _version;
            CLRVersion = RuntimeInformation.FrameworkDescription;
            OSVersion = RuntimeInformation.OSDescription;
            TotalDuration = healthReport.TotalDuration;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                LinuxVersion = GetLinuxVersion.GetLinuxVersionInfo().VersionString;
            }
            else
            {
                LinuxVersion = "N/A";
            }
            var resultDictionary = new Dictionary<string, HealthReportEntryResult<T>>();
            foreach (var result in healthReport.Entries)
            {
                resultDictionary.Add(result.Key, new HealthReportEntryResult<T>(result.Value));
            }
            Results = resultDictionary;
        }
        public readonly string Status;
        public readonly string ApplicationVersion;
        public readonly string CLRVersion;
        public readonly string OSVersion;
        public readonly string LinuxVersion;
        public readonly TimeSpan TotalDuration;
        public readonly IReadOnlyDictionary<string, HealthReportEntryResult<T>> Results;

    }
}
