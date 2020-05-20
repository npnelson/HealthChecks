using System;

namespace NetToolBox.HealthChecks.AzureFunctionTimer
{
    internal sealed class TimerHealthCheckStatus
    {
        public DateTimeOffset LastCheckpoint { get; set; }
    }
}
