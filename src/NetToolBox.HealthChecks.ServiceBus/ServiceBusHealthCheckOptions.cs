namespace NetToolBox.HealthChecks.ServiceBus
{
    public sealed class ServiceBusHealthCheckOptions
    {
        public int ActiveMessageCountThreshold { get; set; }
        public int DeadletterMessageCountThreshold { get; set; }
    }
}
