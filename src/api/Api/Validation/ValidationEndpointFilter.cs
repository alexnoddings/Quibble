using System.Reflection;
using FluentValidation;

namespace Quibble.Api.Validation;

public static class ValidationEndpointFilter
{
    public static TBuilder AddValidationFilters<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilterFactory(Factory);
        return builder;
    }

    private static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        var parameters = factoryContext.MethodInfo.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (parameter.GetCustomAttribute<ValidateAttribute>() is null)
                continue;

            next = CreateParameterValidationDelegate(next, i, parameter.ParameterType);
        }

        return next;
    }

    private static EndpointFilterDelegate CreateParameterValidationDelegate(EndpointFilterDelegate next, int parameterIndex, Type parameterType)
    {
        var methodInfo = _createParameterValidationDelegateGenericMethodInfo.MakeGenericMethod(parameterType);
        var delegateObj = methodInfo.Invoke(null, [next, parameterIndex]);
        return (EndpointFilterDelegate)delegateObj!;
    }

    private static readonly MethodInfo _createParameterValidationDelegateGenericMethodInfo =
        typeof(ValidationEndpointFilter)
            .GetMethod(
                nameof(CreateParameterValidationDelegateGeneric),
                BindingFlags.Static | BindingFlags.NonPublic
            )!;
    private static EndpointFilterDelegate CreateParameterValidationDelegateGeneric<T>(EndpointFilterDelegate next, int parameterIndex) =>
        context => ValidateParameterAsync<T>(context, parameterIndex, next);

    private static async ValueTask<object?> ValidateParameterAsync<T>(
        EndpointFilterInvocationContext context,
        int parameterIndex,
        EndpointFilterDelegate next
    )
    {
        var argument = context.GetArgument<T?>(parameterIndex);
        if (argument is null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return null;
        }

        var validators = context.HttpContext.RequestServices.GetRequiredService<IEnumerable<IValidator<T>>>();
        foreach (var validator in validators)
        {
            var validationResult = await validator.ValidateAsync(argument);
            if (validationResult.IsValid)
                continue;

            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return null;
        }

        return await next(context);
    }
}
