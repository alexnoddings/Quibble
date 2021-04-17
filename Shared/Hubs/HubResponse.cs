namespace Quibble.Shared.Api
{
    public record HubResponse(bool WasSuccessful, string? ErrorCode = null);
    public record HubResponse<TValue>(bool WasSuccessful, string? ErrorCode = null, TValue? Value = default) : HubResponse(WasSuccessful, ErrorCode);
}
