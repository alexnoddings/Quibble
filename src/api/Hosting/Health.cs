using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Quibble.Hosting;

public static class HealthExtensions
{
    private const string HealthCheckPolicy = "HealthChecks";

    public static TBuilder AddQuibbleHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddFilter("Microsoft.Extensions.Diagnostics.HealthChecks", LogLevel.Information);

        builder.Services
            .AddHealthChecks()
            // Always responds as healthy, used to check if app is alive
            .AddCheck("alive", () => HealthCheckResult.Healthy(), ["live"]);

        // If a health check takes longer than 4 seconds, assume the system is probably unhealthy
        builder.Services.AddRequestTimeouts(static timeouts =>
            timeouts.AddPolicy(HealthCheckPolicy, TimeSpan.FromSeconds(4))
        );

        return builder;
    }

    public static WebApplication MapQuibbleHealthEndpoints(this WebApplication app)
    {
        var health = app.MapGroup("/health");
        health.WithRequestTimeout(HealthCheckPolicy);

        // Don't require auth during local dev for Aspire
        if (app.Environment.Is(Env.Development, Env.AutomatedTesting))
            health.AllowAnonymous();
        else
            health.RequireAuthorization();

        // Healthy if all health checks are passing
        health.MapHealthChecks("/healthy", new HealthCheckOptions
        {
            ResponseWriter = WriteHealthResponseAsync
        });

        // Checks if the app is alive
        health.MapGet("/alive", WriteAliveResponseAsync);

        return app;
    }

    private static readonly byte[] _aliveBytes = "alive"u8.ToArray();
    private static Task WriteAliveResponseAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "text/plain";
        return httpContext.Response.Body.WriteAsync(_aliveBytes).AsTask();
    }

    private static readonly JsonWriterOptions _jsonWriterOptions = new() { Indented = true };
    private static Task WriteHealthResponseAsync(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, _jsonWriterOptions))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", GetHealthStatus(healthReport.Status));

            jsonWriter.WriteStartObject("results");
            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status", GetHealthStatus(healthReportEntry.Value.Status));
                jsonWriter.WriteString("description",healthReportEntry.Value.Description);
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        // Not very efficient to convert our memory stream to a string, then write it back to a stream, but it's easy
        var jsonBytes = memoryStream.ToArray();
        var jsonString = Encoding.UTF8.GetString(jsonBytes);
        return context.Response.WriteAsync(jsonString);
    }

    private static string GetHealthStatus(HealthStatus status) =>
        status.ToString().ToLowerInvariant();
}
