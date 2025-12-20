using Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("database");

            return services;
        }
    }

    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _dbContext;

        public DatabaseHealthCheck(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

                return canConnect
                    ? HealthCheckResult.Healthy("Database connection is healthy")
                    : HealthCheckResult.Unhealthy("Cannot connect to database");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database health check failed", ex);
            }
        }
    }
}