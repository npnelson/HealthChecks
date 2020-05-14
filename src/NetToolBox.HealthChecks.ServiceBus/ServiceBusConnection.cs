namespace NetToolBox.HealthChecks.ServiceBus
{
    internal sealed class ServiceBusConnection
    {
        public string ServiceBusConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
