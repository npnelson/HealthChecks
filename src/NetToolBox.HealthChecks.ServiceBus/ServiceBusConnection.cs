namespace NetToolBox.HealthChecks.ServiceBus
{
    internal sealed class ServiceBusConnection
    {
        public string ServiceBusConnectionString { get; set; } = null!;
        public string QueueName { get; set; } = null!;
    }
}
