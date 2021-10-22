namespace Quibble.Client.Services.Authentication;

public sealed class AuthenticationOperation<TResult> : AuthenticationOperation
{
    public TResult? Result { get; set; }

    private AuthenticationOperation(bool wasSuccessful, List<string>? errors, TResult? result) : base(wasSuccessful, errors)
    {
        Result = result;
    }

    public static AuthenticationOperation<TResult> FromSuccess(TResult? result) =>
        new(true, null, result);

    public new static AuthenticationOperation<TResult> FromError(IEnumerable<string> errors) =>
        new(false, errors.ToList(), default);
}
