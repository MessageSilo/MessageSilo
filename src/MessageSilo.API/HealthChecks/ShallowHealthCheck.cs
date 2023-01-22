using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MessageSilo.API.HealthChecks
{
    public class ShallowHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
