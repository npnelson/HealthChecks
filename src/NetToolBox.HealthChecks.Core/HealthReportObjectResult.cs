using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NetToolBox.HealthChecks.Core
{
    public static class HealthReportObjectResult
    {
        public static ObjectResult GetHealthReportObjectResult(HealthReport report)
        {
            var reportResult = new HealthReportResult(report);
            var result = new ObjectResult(reportResult);
            if (report.Status == HealthStatus.Degraded) result.StatusCode = 598;
            if (report.Status == HealthStatus.Unhealthy) result.StatusCode = 599;
            return result;
        }
    }
}
