using Microsoft.Azure.ServiceBus.Management;
using System.Threading.Tasks;

namespace NetToolBox.HealthChecks.ServiceBus
{
    interface IMessageCountDetails
    {
        Task<MessageCountDetails> GetMessageCountDetailsAsync(string connectionString, string queuePath);
    }
}
