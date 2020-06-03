using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NetToolBox.HealthChecks.AzureFunctionTimer;
using System;
using System.Threading.Tasks;

namespace NetToolBox.HealthChecks.Functions.TestHarness
{
    public class TimerTriggerFunction
    {
        private readonly TimerHealthCheckHelper _timerHelper;

        public TimerTriggerFunction(TimerHealthCheckHelper timerHelper)
        {
            _timerHelper = timerHelper;
        }

        [FunctionName("TimerTrigger")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {

            // var temp = CrontabSchedule.Parse("0 */1 * * * *", new ParseOptions { IncludingSeconds = true });


            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            await _timerHelper.CheckpointMethodAsync<TimerTriggerFunction>();
        }
    }


}
