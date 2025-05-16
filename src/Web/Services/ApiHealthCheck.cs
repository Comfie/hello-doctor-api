using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HelloDoctorApi.Web.Services;

public class ApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            HealthCheckResult.Healthy("A healthy resultLocal."));
    }
}