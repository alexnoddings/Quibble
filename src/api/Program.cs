using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.Http.Diagnostics;
using Quibble;
using Quibble.Api;
using Quibble.Data;
using Quibble.Games.Slug;
using Quibble.Hosting;
using Quibble.Users;

var builder = WebApplication.CreateBuilder(args);
builder
    .AddQuibbleAuthentication()
    .AddQuibbleAuthorization();

builder
    .AddQuibbleTelemetry()
    .AddQuibbleHealthChecks();

builder.AddQuibbleApi();

builder.AddQuibbleCaching();
builder.AddQuibbleDbContext();

{
    // Redaction isn't working
    var atc = new DataClassification("Auth", "AccessToken");
    builder.Logging.EnableRedaction();
    builder.Services.AddRedaction(o =>
    {
        o.SetRedactor<AsteriskRedactor>(atc);
    });
    builder.Services.AddHttpLogging();
    builder.Services.AddHttpLoggingRedaction(op =>
    {
        op.RequestPathParameterRedactionMode = HttpRouteParameterRedactionMode.Strict;
        op.RequestPathLoggingMode = IncomingPathLoggingMode.Formatted;
        op.RouteParameterDataClasses = new Dictionary<string, DataClassification> { { "access_token", atc } };
        op.IncludeUnmatchedRoutes = true;
    });
}

builder.Services.AddScoped<RequiresInitialisationCurrentUserService>();
builder.Services.AddScoped<ICurrentUserService>(static sp => sp.GetRequiredService<RequiresInitialisationCurrentUserService>());
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IGameSlugService, GameSlugService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

var isInDevelopment = app.Environment.Is(Env.Development);
if (isInDevelopment)
{
    var dbcf = app.Services.GetRequiredService<IDbContextFactory<QuibbleDbContext>>();
    await using var dbc = await dbcf.CreateDbContextAsync();
    await dbc.Database.MigrateAsync();

    var ensureExists = async (string uoid, string name) =>
    {
        var uoidg = Guid.Parse(uoid);
        if (await dbc.Users.AnyAsync(u => u.ObjectId == uoidg))
            return;

        dbc.Users.Add(new()
        {
            ObjectId = uoidg,
            DisplayName = name,
        });
        await dbc.SaveChangesAsync();
    };
}
else
{
    app.UseExceptionHandler();
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors(static cors =>
    cors.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin()
);

app.UseResponseCompression();

app.UseRouting();

if (isInDevelopment)
    app.MapOpenApi();

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapQuibbleHealthEndpoints();

app.UseContextInjector();

app.MapQuibbleApi();

app.Run();

#pragma warning disable CA1050
public sealed class AsteriskRedactor : Redactor
{
    private const string Redaction = "***";

    public static AsteriskRedactor Instance { get; } = new();

    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        Redaction.CopyTo(destination);
        return Redaction.Length;
    }

    public override int GetRedactedLength(ReadOnlySpan<char> input) => 3;
}
