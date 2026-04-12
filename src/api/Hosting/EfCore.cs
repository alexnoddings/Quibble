using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quibble.Data;

namespace Quibble.Hosting;

public static class EfCore
{
    public static IHostApplicationBuilder AddQuibbleDbContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddPooledDbContextFactory<QuibbleDbContext>(options =>
        {
            if (builder.Environment.IsEnvironment(Quibble.Env.Development))
                options.EnableSensitiveDataLogging();

            var connectionString = builder.Configuration.GetConnectionString("postgresdb");
            options.UseNpgsql(
                connectionString,
                static db => db.MigrationsHistoryTable("migrations", "entity_framework")
            );

            options.ConfigureWarnings(warnings =>
            {
                // Warns about entities using TPC whose Id is configured with an incompatible database-generated default.
                // NpgsqlSequentialGuidValueGenerator is a compatible value generation strategy across distributed systems.
                warnings.Ignore(20609);
            });
        });

        builder.EnrichNpgsqlDbContext<QuibbleDbContext>(static options =>
            // Doesn't support factory-based health checks, so we add one manually
            options.DisableHealthChecks = true
        );

        builder.Services
            .AddHealthChecks()
            .AddDbContextFactoryCheck<QuibbleDbContext>();

        return builder;
    }

    private static IHealthChecksBuilder AddDbContextFactoryCheck<TContext>(this IHealthChecksBuilder builder, string name = "database")
        where TContext : DbContext
    {
        builder.AddCheck<DbContextFactoryHealthCheck<TContext>>(name);

        return builder;
    }

    private sealed class DbContextFactoryHealthCheck<TContext> : IHealthCheck where TContext : DbContext
    {
        private readonly IDbContextFactory<TContext> _dbContextFactory;

        public DbContextFactoryHealthCheck(IDbContextFactory<TContext> dbContextFactory)
        {
            ArgumentNullException.ThrowIfNull(dbContextFactory);

            _dbContextFactory = dbContextFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                if (await CanConnectToDatabaseAsync(cancellationToken))
                    return HealthCheckResult.Healthy();

                return new HealthCheckResult(context.Registration.FailureStatus, "Database is unavailable.");
            }
            catch (Exception exception)
            {
                return HealthCheckResult.Unhealthy(exception.Message, exception);
            }
        }

        private async Task<bool> CanConnectToDatabaseAsync(CancellationToken cancellationToken)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await dbContext.Database.CanConnectAsync(cancellationToken);
        }
    }
}
