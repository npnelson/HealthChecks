﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NetToolBox.HealthChecks.Functions.TestHarness;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NetToolBox.HealthChecks.Functions.TestHarness
{

    public sealed class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {


            //   builder.Services.AddFunctionsHealthChecks().AddServiceBusHealthCheck<ServiceBusTriggerFunction>();
            //builder.Services.AddBlobStorageFactory(new System.Collections.Generic.List<(string accountName, string containerName)> { ("teststorage", "testcontainer") });

            builder.Services.AddFunctionsHealthChecks().AddBlobStorageHealthChecks();


        }
    }
}
