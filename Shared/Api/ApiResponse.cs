using System.Diagnostics.CodeAnalysis;

namespace Quibble.Shared.Api;

public record ApiResponse
{
    protected readonly bool _wasSuccessful;
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool WasSuccessful => _wasSuccessful;

    public ApiError? Error { get; }

    public ApiResponse(bool wasSuccessful, ApiError? error)
    {
        _wasSuccessful = wasSuccessful;

        if (!wasSuccessful && error is null)
            throw new ArgumentException($"Cannot be null when {nameof(wasSuccessful)} is false.", nameof(error));

        Error = error;
    }

    public static ApiResponse FromSuccess()
    {
        return new ApiResponse(true, null);
    }

    public static ApiResponse<TValue> FromSuccess<TValue>(TValue value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value), "Cannot be null for a successful result.");

        return new ApiResponse<TValue>(true, null, value);
    }

    public static ApiResponse FromError(ApiError error)
    {
        if (error is null)
            throw new ArgumentNullException(nameof(error));

        return new ApiResponse(false, error);
    }

    public static ApiResponse<TValue> FromError<TValue>(ApiError error)
    {
        if (error is null)
            throw new ArgumentNullException(nameof(error));

        return new ApiResponse<TValue>(false, error, default);
    }
}
