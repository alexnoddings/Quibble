using Microsoft.AspNetCore.Authorization;

namespace Quibble.Users;

public static class PolicyBuilderExtensions
{
    private static readonly ValidUserAuthorizationRequirement _requirement = new();

    public static AuthorizationPolicyBuilder RequireValidUser(this AuthorizationPolicyBuilder authorizationPolicyBuilder) =>
        authorizationPolicyBuilder.AddRequirements(_requirement);
}

public sealed class ValidUserAuthorizationRequirement : IAuthorizationRequirement
{
}

internal sealed class ValidUserAuthorizationHandler : AuthorizationHandler<ValidUserAuthorizationRequirement>
{
    private readonly RequiresInitialisationCurrentUserService _currentUserService;
    private readonly IUserService _userService;

    public ValidUserAuthorizationHandler(
        RequiresInitialisationCurrentUserService currentUserService,
        IUserService userService
    )
    {
        _currentUserService = currentUserService;
        _userService = userService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ValidUserAuthorizationRequirement requirement
    )
    {
        var userInfo = await _userService.TryGetUserInfoAsync(context.User);
        if (userInfo is not null)
        {
            _currentUserService.InitialiseCurrentUserInfoAsync(userInfo);
            context.Succeed(requirement);
        }
    }

    public override string ToString() =>
        $"{nameof(ValidUserAuthorizationRequirement)}: Requires a valid user.";
}
