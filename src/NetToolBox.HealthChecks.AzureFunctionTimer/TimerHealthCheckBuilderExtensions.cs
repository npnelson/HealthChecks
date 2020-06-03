using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetToolBox.BlobStorage.Azure;
using NetToolBox.DateTimeService;
using NetToolBox.HealthChecks.AzureFunctionTimer;
using NETToolBox.BlobStorage.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TimerHealthCheckBuilderExtensions
    {
        /// <summary>
        /// Scans assembly contained by type T for timer triggers and adds them to a timer healthcheck
        /// Note that you must also accept a TimerHealthCheckHelper in your timer trigger via constructor injection and call TimerHealthCheckHelper.CheckpointMethod passing in the type of your timer so that it writes the checkpoint to blob storage"
        /// This constructor uses a default tolerance timespan of 5 minutes
        /// </summary>
        /// <typeparam name="T">Any type from the assembly you want to scan for timertriggers</typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHealthChecksBuilder AddTimerTriggerHealthCheck<T>(this IHealthChecksBuilder builder)
        {
            return builder.AddTimerTriggerHealthCheck<T>(new TimerTriggerHealthCheckOptions { ToleranceTimeSpan = TimeSpan.FromMinutes(5) });
        }

        /// <summary>
        ///  /// Scans assembly contained by type T for timer triggers and adds them to a timer healthcheck
        /// Note that you must also accept a TimerHealthCheckHelper in your timer trigger via constructor injection and call TimerHealthCheckHelper.CheckpointMethod passing in the type of your timer so that it writes the checkpoint to blob storage"
        /// 
        /// </summary>
        /// <typeparam name="T">Any type from the assembly you want to scan for timertriggers</typeparam>
        /// <param name="builder"></param>
        /// <param name="options">Options to configure the timer healthcheck</param>
        /// <returns></returns>
        public static IHealthChecksBuilder AddTimerTriggerHealthCheck<T>(this IHealthChecksBuilder builder, TimerTriggerHealthCheckOptions options)
        {
            List<TimerTriggerInfo> timerTriggerInfos = new List<TimerTriggerInfo>();
            builder.Services.TryAddSingleton<TimerHealthCheckHelper>();
            builder.Services.AddAzureBlobStorageFactory();
            builder.Services.AddDateTimeService();
            var sp = builder.Services.BuildServiceProvider();
            var config = sp.GetRequiredService<IConfiguration>();
            var connString = config.GetWebJobsConnectionString("AzureWebJobsStorage");
            var isProduction = Environment.GetEnvironmentVariable("APPSETTING_WEBSITE_SLOT_NAME") == "Production";
            var siteName = Environment.GetEnvironmentVariable("APPSETTING_WEBSITE_SITE_NAME") ?? string.Empty;
            var checkhelperOptions = new TimerHealthCheckHelperOptions { AzureWebJobsStorageConnectionString = connString, IsProductionSlot = isProduction, AzureWebSiteName = siteName };

            builder.Services.AddSingleton(checkhelperOptions);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Assembly assem = Assembly.GetAssembly(typeof(T));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (assem == null) throw new InvalidOperationException("Could not find assembly for type. This should never happen");
            var types = assem.GetTypes().Where(x => x.GetMethods().SelectMany(x => x.GetParameters()).Any(x => x.GetCustomAttribute<TimerTriggerAttribute>() != null));
            foreach (Type type in types)
            {
                var functionName = type.GetMethods().Select(x => x.GetCustomAttribute<FunctionNameAttribute>()).Single(x => x != null);
                var trigger = type.GetMethods().SelectMany(x => x.GetParameters()).SelectMany(x => x.GetCustomAttributes<TimerTriggerAttribute>()).Select(x => new TimerTriggerInfo { TimerFullTypeName = type.FullName ?? string.Empty, ScheduleExpression = x.ScheduleExpression, IsTimerDisabled = Environment.GetEnvironmentVariable($"APPSETTING_AzureWebJobs_{functionName!.Name}_Disabled") == "1" }).Single();


                timerTriggerInfos.Add(trigger);
            }

            builder.AddCheck("TimerTrigger", new TimerTriggerHealthCheck(sp.GetRequiredService<IBlobStorageFactory>(), sp.GetRequiredService<IDateTimeService>(), options, checkhelperOptions, timerTriggerInfos));
            return builder;
        }
    }
}
