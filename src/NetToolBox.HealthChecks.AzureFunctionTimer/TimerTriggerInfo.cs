namespace NetToolBox.HealthChecks.AzureFunctionTimer
{
    internal sealed class TimerTriggerInfo
    {
        public string TimerFullTypeName { get; set; } = null!;
        public string ScheduleExpression { get; set; } = null!;
        public bool IsTimerDisabled { get; set; }
        public string TimerTriggerFriendlyName { get; set; } = null!;
    }
}
