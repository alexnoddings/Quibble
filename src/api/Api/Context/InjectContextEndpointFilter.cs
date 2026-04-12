namespace Quibble.Api.Context;

[AttributeUsage(AttributeTargets.Method)]
public class InjectContextAttribute : Attribute;

internal sealed class InjectContextMetadata
{
    public static readonly InjectContextMetadata InjectContext = new();

    private InjectContextMetadata() { }
}

public static class InjectContextExtensions
{
    public static TBuilder InjectContext<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        // Run once all endpoints have been registered
        builder.Finally(static builder =>
        {
            // Check if any of the metadata on the endpoint inherit from InjectContextAttribute
            // (from a [RequireXyzAttribute] on the method)
            if (builder.Metadata.Any(m => m is InjectContextAttribute))
                builder.Metadata.Add(InjectContextMetadata.InjectContext);
        });
        return builder;
    }
}
