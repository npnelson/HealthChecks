using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetToolBox.HealthChecks.Core;
using System.Threading.Tasks;

namespace NetToolBox.HealthChecks.Functions.TestHarness
{
    public class Health
    {
        private readonly HealthCheckService _healthCheckService;

        public Health(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }
        [FunctionName("Health")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req
            )
        {
            var healthReport = await _healthCheckService.CheckHealthAsync();
            return HealthReportObjectResult.GetHealthReportObjectResult(healthReport);
        }
    }
}
