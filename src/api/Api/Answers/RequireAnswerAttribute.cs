using Quibble.Api.Context;

namespace Quibble.Api.Answers;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireAnswerAttribute : InjectContextAttribute;
