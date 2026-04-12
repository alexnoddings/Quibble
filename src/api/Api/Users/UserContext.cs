using Quibble.Users;

namespace Quibble.Api.Users;

public sealed class UserContext
{
    public UserInfo User { get; }

    public UserContext(UserInfo user)
    {
        User = user;
    }

    public static async ValueTask<UserContext?> BindAsync(HttpContext context)
    {
        var userInfoService = context.RequestServices.GetRequiredService<ICurrentUserService>();
        var userInfo = await userInfoService.GetCurrentUserInfoAsync();

        return new UserContext(userInfo);
    }
}
