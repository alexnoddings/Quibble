using Quibble.Api.Context;

namespace Quibble.Api.Rounds;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireRoundAttribute : InjectContextAttribute;
