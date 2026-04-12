namespace Quibble.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGetCurrentUser();

        return endpoints;
    }
}
