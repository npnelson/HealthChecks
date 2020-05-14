namespace NetToolBox.HealthChecks.ServiceBus
{
    public sealed class ServiceBusConnectionHealthResult
    {
        public long ActiveMessageCount { get; set; }
        public long DeadLetterMessageCount { get; set; }
    }
}
