using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetToolBox.HealthChecks.ServiceBus;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBusHealthCheckHealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddServiceBusHealthCheck<T>(this IHealthChecksBuilder builder)
        {
            return builder.AddServiceBusHealthCheck<T>(new ServiceBusHealthCheckOptions { ActiveMessageCountThreshold = 5, DeadletterMessageCountThreshold = 0 });
        }

        public static IHealthChecksBuilder AddServiceBusHealthCheck<T>(this IHealthChecksBuilder builder, ServiceBusHealthCheckOptions options)

        {
            builder.Services.TryAddSingleton<IMessageCountDetails, MessageCountDetailClient>();
            var sp = builder.Services.BuildServiceProvider();
            var nm = sp.GetRequiredService<INameResolver>();
            var config = sp.GetRequiredService<IConfiguration>();

            var assem = Assembly.GetAssembly(typeof(T));
            var connections = assem.GetTypes().SelectMany(x => x.GetMethods()).SelectMany(x => x.GetParameters()).SelectMany(x => x.GetCustomAttributes<ServiceBusTriggerAttribute>()).Select(x => new ServiceBusConnection { ServiceBusConnectionString = config.GetWebJobsConnectionString(x.Connection), QueueName = nm.ResolveWholeString(x.QueueName) }).ToList();


            builder.Services.TryAddSingleton<IMessageCountDetails, MessageCountDetailClient>();
            builder.AddCheck("ServiceBus", new ServiceBusHealthCheck(sp.GetRequiredService<IMessageCountDetails>(), connections, options));
            return builder;
        }
    }
}
