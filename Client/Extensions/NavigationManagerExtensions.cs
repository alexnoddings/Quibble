using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace BlazorIdentityBase.Client.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static T GetQueryParameter<T>(this NavigationManager navigationManager, string parameterName, T? defaultValue = default)
        [return: NotNullIfNotNull("defaultValue")]
        public static string? GetQueryParameter(this NavigationManager navigationManager, string parameterName, string? defaultValue = null)
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

            var stringValue = QueryHelpers.ParseQuery(uri.Query).GetValueOrDefault(parameterName, StringValues.Empty);
            if (stringValue.Equals(StringValues.Empty))
            var value = QueryHelpers.ParseQuery(uri.Query).GetValueOrDefault(parameterName, StringValues.Empty);
            if (value.Equals(StringValues.Empty))
                return defaultValue;

            return JsonSerializer.Deserialize<T>(stringValue);
            return value;
        }
        }
    }
}
