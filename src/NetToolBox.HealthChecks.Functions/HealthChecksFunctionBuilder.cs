using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace NetToolBox.HealthChecks.Functions
{
    internal sealed class HealthChecksFunctionBuilder : IHealthChecksBuilder
    {
        public HealthChecksFunctionBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public IHealthChecksBuilder Add(HealthCheckRegistration registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            Services.Configure<HealthCheckServiceOptions>(options =>
            {
                options.Registrations.Add(registration);
            });

            return this;
        }
    }
}
