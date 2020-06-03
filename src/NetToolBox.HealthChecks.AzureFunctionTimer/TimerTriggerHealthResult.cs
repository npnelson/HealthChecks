using System;

namespace NetToolBox.HealthChecks.AzureFunctionTimer
{
    public sealed class TimerTriggerHealthResult
    {
        public DateTimeOffset LastCompletionTime { get; set; }
        public DateTimeOffset LastExpectedCompletionTime { get; set; }
        public bool IsTimerDisabled { get; set; }
    }
}
