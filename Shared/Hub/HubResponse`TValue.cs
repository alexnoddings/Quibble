using System;
using System.Diagnostics.CodeAnalysis;
using Quibble.Shared.Resources;

namespace Quibble.Shared.Hub
{
    public record HubResponse
    {
        [MemberNotNullWhen(false, nameof(ErrorCode))]
        public virtual bool WasSuccessful { get; }
        public string? ErrorCode { get; }

        public HubResponse(bool wasSuccessful, string? errorCode)
        {
            if (!wasSuccessful && string.IsNullOrWhiteSpace(errorCode))
                throw new ArgumentException(nameof(errorCode));

            WasSuccessful = wasSuccessful;
            ErrorCode = errorCode;
        }

        public static HubResponse FromSuccess()
        {
            return new HubResponse(true, null);
        }

        public static HubResponse<TValue> FromSuccess<TValue>(TValue value)
        {
            if (value is null)
                return new HubResponse<TValue>(false, nameof(ErrorMessages.UnknownError), default);

            return new HubResponse<TValue>(true, null, value);
        }

        public static HubResponse FromError(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
                errorCode = nameof(ErrorMessages.UnknownError);

            return new HubResponse(false, errorCode);
        }

        public static HubResponse<TValue> FromError<TValue>(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
                errorCode = nameof(ErrorMessages.UnknownError);

            return new HubResponse<TValue>(false, errorCode, default);
        }
    }
    public record HubResponse<TValue> : HubResponse
    {
        [MemberNotNullWhen(true, nameof(Value))]
        public override bool WasSuccessful { get; }
        public TValue? Value { get; }

        public HubResponse(bool wasSuccessful, string? errorCode, TValue? value)
            : base(wasSuccessful, errorCode)
        {
            if (wasSuccessful && value is null)
                throw new ArgumentNullException(nameof(value));

            WasSuccessful = wasSuccessful;
            Value = value;
        }
    }
}
