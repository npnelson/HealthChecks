using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace NetToolBox.HealthChecks.Functions.TestHarness
{
    public class ServiceBusTriggerFunction
    {

        [Disable]
        [FunctionName("ServiceBusTriggerFunction")]
        public void Run([ServiceBusTrigger("%ServiceBusSettings:QueueName%", Connection = "ServiceBusSettings:ConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
