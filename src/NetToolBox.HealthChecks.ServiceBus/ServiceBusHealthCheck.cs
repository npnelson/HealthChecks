using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetToolBox.HealthChecks.ServiceBus
{
    internal sealed class ServiceBusHealthCheck : IHealthCheck
    {
        private readonly IMessageCountDetails _messageCountDetails;
        private readonly List<ServiceBusConnection> _connections;
        private readonly ServiceBusHealthCheckOptions _options;

        public ServiceBusHealthCheck(IMessageCountDetails messageCountDetails, List<ServiceBusConnection> connections, ServiceBusHealthCheckOptions options)
        {
            _messageCountDetails = messageCountDetails;
            _connections = connections;
            _options = options;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {

            var itemDictionary = new Dictionary<string, object>();
            HealthCheckResult retval;

            try
            {
                foreach (var connection in _connections)
                {
                    var messageCount = await _messageCountDetails.GetMessageCountDetailsAsync(connection.ServiceBusConnectionString, connection.QueueName);

                    itemDictionary.Add($"{connection.ServiceBusConnectionString}:{connection.QueueName}", new ServiceBusConnectionHealthResult { ActiveMessageCount = messageCount.ActiveMessageCount, DeadLetterMessageCount = messageCount.DeadLetterMessageCount });
                }
                if (itemDictionary.Values.Any(x => ((ServiceBusConnectionHealthResult)x).ActiveMessageCount > _options.ActiveMessageCountThreshold || itemDictionary.Values.Any(x => ((ServiceBusConnectionHealthResult)x).DeadLetterMessageCount > _options.DeadletterMessageCountThreshold)))
                {
                    retval = new HealthCheckResult(HealthStatus.Unhealthy, $"Active messages exceed active threshold count of {_options.ActiveMessageCountThreshold} and/or Deadletter message count exceeds deadletter threshold count of {_options.DeadletterMessageCountThreshold}", null, itemDictionary);
                }
                else
                {
                    retval = new HealthCheckResult(HealthStatus.Healthy, null, null, itemDictionary);
                }
            }
            catch (System.Exception ex)
            {
                retval = new HealthCheckResult(HealthStatus.Unhealthy, "", ex, itemDictionary);
            }

            return retval;
        }
    }
}
