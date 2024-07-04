using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiBaseTemplate.Web.Services
{
    public class ApiHealthCheck : IHealthCheck
    {

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) => Task.FromResult(
               HealthCheckResult.Healthy("A healthy result."));
    }
}
