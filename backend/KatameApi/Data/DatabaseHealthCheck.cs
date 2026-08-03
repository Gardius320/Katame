using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KatameApi.Data;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly KatameDbContext _context;

    public DatabaseHealthCheck(KatameDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
        return canConnect
            ? HealthCheckResult.Healthy("MySQL disponible.")
            : HealthCheckResult.Unhealthy("No se pudo conectar a MySQL.");
    }
}
