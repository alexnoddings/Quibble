namespace Quibble.Api.Users;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireUserOwnsGameAttribute : Attribute
{
}
