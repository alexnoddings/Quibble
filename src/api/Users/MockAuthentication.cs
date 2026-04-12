using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Quibble.Users;

public static class MockAuthDefaults
{
    public const string AuthenticationScheme = "Mock";
}

public class MockAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
}

public class MockAuthenticationHandler : AuthenticationHandler<MockAuthenticationSchemeOptions>
{
    public MockAuthenticationHandler(IOptionsMonitor<MockAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    // [Obsolete]
    // public MockAuthenticationHandler(IOptionsMonitor<MockAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    // {
    // }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = HandleAuthenticate(Context);
        return Task.FromResult(result);
    }

    private static AuthenticateResult HandleAuthenticate(HttpContext httpContext)
    {
        var userObjectId = TryGetMockUserObjectId(httpContext);
        if (userObjectId is null)
            return AuthenticateResult.NoResult();

        var identity = CreateClaimsIdentity(userObjectId.Value);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, MockAuthDefaults.AuthenticationScheme);
        return AuthenticateResult.Success(ticket);
    }

    private static Guid? TryGetMockUserObjectId(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-Quibble-User-ObjectId", out var objectIdValues))
            return null;

        if (objectIdValues.Count == 0)
            return null;

        var objectIdStr = objectIdValues[0];
        if (!Guid.TryParse(objectIdStr, out var objectId))
            return null;

        return objectId;
    }

    private const string ObjectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private static ClaimsIdentity CreateClaimsIdentity(Guid objectId)
    {
        var identity = new ClaimsIdentity(MockAuthDefaults.AuthenticationScheme);

        var objectIdClaim = new Claim(ObjectIdClaimType, objectId.ToString());
        identity.AddClaim(objectIdClaim);

        var userNameClaim = new Claim(ClaimTypes.Name, "Mock user");
        identity.AddClaim(userNameClaim);

        return identity;
    }
}
