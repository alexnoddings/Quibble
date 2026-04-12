using Aspire.Hosting.DevTunnels;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = AddPostgresServer(builder);
var postgresdb = postgres.AddDatabase("postgresdb");

var redis = builder
    .AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

redis.WithRedisCommander(cfg =>
{
    cfg.WithLifetime(ContainerLifetime.Persistent);
    cfg.WithParentRelationship(redis);
});

var api = builder
    .AddProject<Projects.Api>("quibble")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithReference(postgresdb).WaitFor(postgresdb)
    .WithReference(redis).WaitFor(redis)
    .WithHttpHealthCheck("/health/healthy")
    .WithHttpHealthCheck("/health/alive");

var client = builder
    .AddJavaScriptApp("client", "../client", runScriptName: "start")
    .WithBun()
    .WithReference(api).WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .PublishAsDockerFile()
    .WithIconName("Globe");

var gateway = builder.AddYarp("gateway")
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute("/api/{**catch-all}", api);
        yarp.AddRoute("/health/{**catch-all}", api);
        yarp.AddRoute(client);
    })
    .WithEndpoint("https", ep =>
    {
        ep.Port = 8080;
        ep.UriScheme = Uri.UriSchemeHttps;
    })
    .WithIconName("OrganizationHorizontal")
    .WithUrlForEndpoint("https", endpoint => new ResourceUrlAnnotation
    {
        DisplayOrder = 101,
        DisplayText = "quibble",
        Url = $"{endpoint.Scheme}://quibble.dev.localhost:{endpoint.Port}/",
        Endpoint = endpoint,
    })
    .WithLifetime(ContainerLifetime.Persistent);

var devTunnelOptions = new DevTunnelOptions
{
    Description = "Quibble development environment",
    Labels = ["dev"],
    AllowAnonymous = true
};

var tunnel = builder
    .AddDevTunnel("dev-tunnel", "quibble-tunnel", devTunnelOptions)
    .WithReference(gateway)
    .WithExplicitStart();

await builder.Build().RunAsync();
return;

static IResourceBuilder<PostgresServerResource> AddPostgresServer(IDistributedApplicationBuilder builder)
{
    string name;
    IResourceBuilder<ParameterResource>? password;

    if (builder.Environment.EnvironmentName is "AutomatedTesting")
    {
        name = "postgres-testing";
        password = builder.AddParameter(
            "postgresPassword",
            secret: true,
            // Only used for integration testing, so doesn't need to be stored securely
            value: "Br5hpJs2V0hYxsFbIlLhpphf"
        );
    }
    else
    {
        name = "postgres";
        password = null;
    }

    var postgres =
        builder
            .AddPostgres(name, password)
            .WithDataVolume()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithHostPort(8090)
            .WithEndpointProxySupport(false)
            .WithIconName("DatabaseMultiple");

    postgres.WithPgWeb(cfg =>
    {
        cfg.WithHostPort(8091);
        cfg.WithLifetime(ContainerLifetime.Persistent);

        cfg.WithParentRelationship(postgres);
        cfg.WithIconName("DatabaseSearch");
    });

    return postgres;
}
