using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MessageSilo.HealthChecks
{
    public class DeepHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            //TODO: A deep health check
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
