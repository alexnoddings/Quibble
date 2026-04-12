using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Quibble.Data;

namespace Quibble.Users;

public class UserInfo
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
}

public interface IUserService
{
    public Task<UserInfo?> TryGetUserInfoAsync(Guid userObjectId);
    public Task<UserInfo?> TryGetUserInfoAsync(ClaimsPrincipal user);
}

internal sealed class UserService : IUserService
{
    private readonly IUserInfoCache _cache;
    private readonly IDbContextFactory<QuibbleDbContext> _dbContextFactory;

    public UserService(
        IUserInfoCache cache,
        IDbContextFactory<QuibbleDbContext> dbContextFactory
    )
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
    }

    private static readonly Task<UserInfo?> _notFoundUserTask = Task.FromResult<UserInfo?>(null);
    public Task<UserInfo?> TryGetUserInfoAsync(ClaimsPrincipal user)
    {
        var userObjectId = TryGetUserObjectId(user);
        if (userObjectId is null)
            return _notFoundUserTask;

        return TryGetUserInfoAsync(userObjectId.Value);
    }

    public async Task<UserInfo?> TryGetUserInfoAsync(Guid userObjectId)
    {
        var userInfo = await _cache.GetOrCreateUserAsync(userObjectId, LoadUserRawAsync);
        return userInfo;
    }

    private static Guid? TryGetUserObjectId(ClaimsPrincipal user)
    {
        const string objectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        var objectIdStr = user.Claims.FirstOrDefault(c => c.Type == objectIdClaimType)?.Value;
        if (objectIdStr is null)
            return null;

        if (!Guid.TryParse(objectIdStr, out var objectId))
            return null;

        return objectId;
    }

    private async ValueTask<UserInfo?> LoadUserRawAsync(Guid objectId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var user = await dbContext.Users
            .Where(u => u.ObjectId == objectId)
            .Select(u => new UserInfo { Id = u.Id, DisplayName = u.DisplayName })
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}

public interface IUserInfoCache
{
    public ValueTask<UserInfo?> GetOrCreateUserAsync(Guid objectId, Func<Guid, CancellationToken, ValueTask<UserInfo?>> create);
}

public class UserInfoCache : IUserInfoCache
{
    private readonly HybridCache _cache;

    public UserInfoCache(
        HybridCache cache
    )
    {
        _cache = cache;
    }

    private static readonly HybridCacheEntryOptions _cacheOptions = new()
    {
        LocalCacheExpiration = TimeSpan.FromSeconds(45),
        Expiration = TimeSpan.FromSeconds(90)
    };

    public ValueTask<UserInfo?> GetOrCreateUserAsync(Guid objectId, Func<Guid, CancellationToken, ValueTask<UserInfo?>> create)
    {
        var key = $"userinfo:objectid:{objectId:N}";
        return _cache.GetOrCreateAsync(
            key,
            objectId,
            create,
            _cacheOptions
        );
    }
}
