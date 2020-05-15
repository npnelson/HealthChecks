using NetToolBox.HealthChecks.BlobStorage;
using NETToolBox.BlobStorage.Abstractions;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlobStorageHealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddBlobStorageHealthChecks(this IHealthChecksBuilder builder)
        {
            if (!builder.Services.Any(x => x.ServiceType == typeof(IBlobStorageFactory))) throw new InvalidOperationException("You must register an implementation for IBlobStorageFactory before calling AddBlobStorageHealthChecks"); //we don't want to register one here because we don't know what implementation they might want to use

            builder.AddCheck<BlobStorageHealthCheck>("BlobStorage");
            return builder;
        }
    }
}
