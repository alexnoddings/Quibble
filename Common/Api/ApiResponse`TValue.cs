using System.Diagnostics.CodeAnalysis;

namespace Quibble.Common.Api;

public record ApiResponse<TValue> : ApiResponse
{
	[MemberNotNullWhen(true, nameof(Value))]
	public override bool WasSuccessful => _wasSuccessful;

	public TValue? Value { get; }

	public ApiResponse(bool wasSuccessful, ApiError? error, TValue? value)
		: base(wasSuccessful, error)
	{
		if (wasSuccessful && value == null)
			throw new ArgumentNullException(nameof(value), $"Cannot be null when {nameof(wasSuccessful)} is true.");

		Value = value;
	}
}
