namespace Quibble.Shared.Api
{
    public class ApiResponse<TResult> : ApiResponse
    {
        public TResult? Result { get; init; }
    }
}
