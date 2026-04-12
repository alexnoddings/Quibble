namespace Quibble.Users;

public interface ICurrentUserService
{
    public ValueTask<UserInfo> GetCurrentUserInfoAsync();
}

internal sealed class RequiresInitialisationCurrentUserService : ICurrentUserService
{
    private UserInfo? _userInfo;

    public ValueTask<UserInfo> GetCurrentUserInfoAsync()
    {
        var userInfo = _userInfo;
        if (userInfo is null)
            throw new InvalidOperationException("User has not been initialised.");

        return ValueTask.FromResult(userInfo);
    }

    public void InitialiseCurrentUserInfoAsync(UserInfo userInfo)
    {
        _userInfo = userInfo;
    }
}
