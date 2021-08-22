using System.Diagnostics.CodeAnalysis;

namespace Quibble.Shared.Api
{
    public record ApiResponse<TValue> : ApiResponse
    {
        [MemberNotNullWhen(true, nameof(Value))]
        public override bool WasSuccessful { get; }

        public TValue? Value { get; }

        public ApiResponse(bool wasSuccessful, ApiError? error, TValue? value)
            : base(wasSuccessful, error)
        {
            if (wasSuccessful && value is null)
                throw new ArgumentNullException(nameof(value), $"Cannot be null when {nameof(wasSuccessful)} is true.");
            if (!wasSuccessful && value is not null)
                throw new ArgumentException($"Must be null when {nameof(wasSuccessful)} is false.", nameof(value));

            WasSuccessful = wasSuccessful;
            Value = value;
        }
    }
}
