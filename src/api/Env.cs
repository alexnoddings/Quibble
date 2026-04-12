namespace Quibble;

public static class Env
{
    public const string Development = nameof(Development);
    public const string AutomatedTesting = nameof(AutomatedTesting);
    public const string Staging = nameof(Staging);
    public const string Production = nameof(Production);
}

public static class HostEnvironmentExtensions
{
    public static bool Is(this IHostEnvironment environment, params ReadOnlySpan<string> environments)
    {
        var environmentName = environment.EnvironmentName;
        foreach (var checkEnvironmentName in environments)
        {
            if (checkEnvironmentName == environmentName)
                return true;
        }

        return false;
    }
}
