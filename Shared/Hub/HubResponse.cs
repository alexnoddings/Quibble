using System;
using System.Diagnostics.CodeAnalysis;

namespace Quibble.Shared.Hub
{
    public record HubResponse
    {
        [MemberNotNullWhen(false, nameof(Error))]
        public virtual bool WasSuccessful { get; }
        public ApiError? Error { get; }

        public HubResponse(bool wasSuccessful, ApiError? error)
        {
            if (!wasSuccessful && error is null)
                throw new ArgumentException($"Cannot be null or whitespace when {nameof(wasSuccessful)} is false.", nameof(error));

            WasSuccessful = wasSuccessful;
            Error = error;
        }

        public static HubResponse FromSuccess()
        {
            return new HubResponse(true, null);
        }

        public static HubResponse<TValue> FromSuccess<TValue>(TValue value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), "Cannot be null for a successful result.");

            return new HubResponse<TValue>(true, null, value);
        }

        public static HubResponse FromError(ApiError error)
        {
            if (error is null)
                throw new ArgumentNullException(nameof(error));

            return new HubResponse(false, error);
        }

        public static HubResponse<TValue> FromError<TValue>(ApiError error)
        {
            if (error is null)
                throw new ArgumentNullException(nameof(error));

            return new HubResponse<TValue>(false, error, default);
        }
    }
}
