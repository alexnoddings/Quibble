namespace Quibble.Common.Api;

public record ApiError(int StatusCode, string ErrorKey);
