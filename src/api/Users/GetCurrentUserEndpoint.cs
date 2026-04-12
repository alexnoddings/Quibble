using System.Text.Json;

namespace Quibble.Users;

public static class GetCurrentUserEndpoint
{
    public static IEndpointRouteBuilder MapGetCurrentUser(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/@me", GetCurrentUserAsync);
        return endpoints;
    }

    private static Task GetCurrentUserAsync(HttpContext http) =>
        JsonSerializer.SerializeAsync(
            http.Response.Body,
            http.User.Identities.Select(ident => new
            {
                ident.Name,
                ident.AuthenticationType,
                Claims = ident.Claims.Select(c => new { c.Type, c.Value }).ToArray()
            })
        );
}
