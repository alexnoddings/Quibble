using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Quibble.Hosting;

public static class Metrics
{
    public const string MeterName = "Quibble";
}

public static class TelemetryExtensions
{
    public static TBuilder AddQuibbleTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(static logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services
            .AddOpenTelemetry()
            .WithQuibbleMetrics()
            .WithQuibbleTracing(builder.Environment);

        builder.AddQuibbleTelemetryExporters();

        return builder;
    }

    private static OpenTelemetryBuilder WithQuibbleMetrics(this OpenTelemetryBuilder builder)
    {
        return builder.WithMetrics(static metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter(Metrics.MeterName)
        );
    }

    private static OpenTelemetryBuilder WithQuibbleTracing(this OpenTelemetryBuilder builder, IHostEnvironment environment)
    {
        var appName = environment.ApplicationName;
        var isDevOrAutomatedTesting =
            environment.IsEnvironment(Env.Development)
            || environment.IsEnvironment(Env.AutomatedTesting);

        return builder.WithTracing(tracing =>
        {
            if (isDevOrAutomatedTesting)
                tracing.SetSampler(new AlwaysOnSampler());

            tracing
                .AddSource(appName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
        });
    }

    private static TBuilder AddQuibbleTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        var useOtlpExporter = !string.IsNullOrWhiteSpace(otlpEndpoint);
        if (useOtlpExporter)
        {
            builder.Services
                .AddOpenTelemetry()
                .UseOtlpExporter();
        }

        return builder;
    }
}
