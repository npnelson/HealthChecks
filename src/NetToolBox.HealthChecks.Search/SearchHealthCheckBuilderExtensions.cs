using Microsoft.Extensions.DependencyInjection;
using NetToolBox.Search.Abstractions;
using System;
using System.Linq;

namespace NetToolBox.HealthChecks.Search
{
    public static class SearchHealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddSearchHealthChecks(this IHealthChecksBuilder builder)
        {
            if (!builder.Services.Any(x => x.ServiceType == typeof(ISearchClient))) throw new InvalidOperationException("You must register an implementation for IBlobStorageFactory before calling AddBlobStorageHealthChecks"); //we don't want to register one here because we don't know what implementation they might want to use

            builder.AddCheck<SearchHealthCheck>("Search");
            return builder;
        }
    }
}