namespace Quibble.Shared.Api
{
    public class ApiResponse
    {
        public bool WasSuccessful { get; init; }
        public string? HumanError { get; init; }
    }
}
