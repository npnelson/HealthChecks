using System;

namespace NetToolBox.HealthChecks.AzureFunctionTimer
{
    public sealed class TimerTriggerHealthCheckOptions
    {
        public TimeSpan ToleranceTimeSpan { get; set; }
    }
}
