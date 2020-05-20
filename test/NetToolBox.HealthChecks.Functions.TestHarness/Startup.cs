using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NetToolBox.HealthChecks.Functions.TestHarness;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NetToolBox.HealthChecks.Functions.TestHarness
{

    public sealed class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddFunctionsHealthChecks().AddTimerTriggerHealthCheck<Startup>(new AzureFunctionTimer.TimerTriggerHealthCheckOptions { ToleranceTimeSpan = TimeSpan.FromMinutes(2) });
            //   builder.Services.AddFunctionsHealthChecks().AddServiceBusHealthCheck<ServiceBusTriggerFunction>();
            //builder.Services.AddBlobStorageFactory(new System.Collections.Generic.List<(string accountName, string containerName)> { ("teststorage", "testcontainer") });

            // builder.Services.AddFunctionsHealthChecks().AddBlobStorageHealthChecks();

            //var sp = builder.Services.BuildServiceProvider();
            //var conn = sp.GetRequiredService<IConfiguration>().GetWebJobsConnectionString(ConnectionStringNames.Storage);  //azure-webjobs-hosts
            //var hostIdProvider = sp.GetRequiredService<IHostIdProvider>();
            //var hostID = hostIdProvider.GetHostIdAsync(CancellationToken.None).GetAwaiter().GetResult();

        }
    }
}
