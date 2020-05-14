using Microsoft.Azure.ServiceBus.Management;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetToolBox.HealthChecks.ServiceBus
{
    internal class MessageCountDetailClient : IMessageCountDetails
    {
        private readonly Dictionary<string, ManagementClient> _managementClientDictionary = new Dictionary<string, ManagementClient>();
        public async Task<MessageCountDetails> GetMessageCountDetailsAsync(string connectionString, string queuePath)
        {
            if (!_managementClientDictionary.ContainsKey(connectionString))
            {
                _managementClientDictionary.Add(connectionString, new ManagementClient(connectionString));
            }
            var managementClient = _managementClientDictionary[connectionString];
            var results = await managementClient.GetQueueRuntimeInfoAsync(queuePath).ConfigureAwait(false);
            return results.MessageCountDetails;
        }
    }
}
