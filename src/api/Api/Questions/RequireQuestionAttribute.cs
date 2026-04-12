using Quibble.Api.Context;

namespace Quibble.Api.Questions;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequireQuestionAttribute : InjectContextAttribute;
