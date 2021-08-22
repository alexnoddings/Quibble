using System.Diagnostics.CodeAnalysis;

namespace Quibble.Shared.Api
{
    public record ApiResponse
    {
        [MemberNotNullWhen(false, nameof(Error))]
        public virtual bool WasSuccessful { get; }

        public ApiError? Error { get; }

        public ApiResponse(bool wasSuccessful, ApiError? error)
        {
            if (!wasSuccessful && error is null)
                throw new ArgumentException($"Cannot be null or whitespace when {nameof(wasSuccessful)} is false.", nameof(error));
            if (wasSuccessful && error is not null)
                throw new ArgumentException($"Must be null or whitespace when {nameof(wasSuccessful)} is true.", nameof(error));

            WasSuccessful = wasSuccessful;
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
}
