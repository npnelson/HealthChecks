namespace NetToolBox.HealthChecks.AzureFunctionTimer
{
    public sealed class TimerHealthCheckHelperOptions
    {
        public string AzureWebJobsStorageConnectionString { get; set; } = null!;
        public bool IsProductionSlot { get; set; }
        public string AzureWebSiteName { get; set; } = null!;
    }
}
