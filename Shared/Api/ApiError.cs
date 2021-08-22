namespace Quibble.Shared.Api
{
    public record ApiError(int StatusCode, string ErrorKey);
}
