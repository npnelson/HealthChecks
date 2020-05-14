using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace NetToolBox.HealthChecks.Functions.TestHarness
{
    public class ServiceBusFunction2
    {
        [Disable]
        [FunctionName("ServiceBusFunction2")]
        public void Run([ServiceBusTrigger("test", Connection = "ServiceBusSettings:ConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
