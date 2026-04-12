using Quibble.Api.Context;

namespace Quibble.Api.Games;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireGameAttribute : InjectContextAttribute;
