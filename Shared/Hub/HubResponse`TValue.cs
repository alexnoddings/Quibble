using System;
using System.Diagnostics.CodeAnalysis;

namespace Quibble.Shared.Hub
{
    public record HubResponse<TValue> : HubResponse
    {
        [MemberNotNullWhen(true, nameof(Value))]
        public override bool WasSuccessful { get; }
        public TValue? Value { get; }

        public HubResponse(bool wasSuccessful, ApiError? error, TValue? value)
            : base(wasSuccessful, error)
        {
            if (wasSuccessful && value is null)
                throw new ArgumentNullException(nameof(value));

            WasSuccessful = wasSuccessful;
            Value = value;
        }
    }
}
