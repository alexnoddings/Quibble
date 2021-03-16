using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace BlazorIdentityBase.Client.Extensions
{
    public static class NavigationManagerExtensions
    {
        [return: NotNullIfNotNull("defaultValue")]
        public static string? GetQueryParameter(this NavigationManager navigationManager, string parameterName, string? defaultValue = null)
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

            var value = QueryHelpers.ParseQuery(uri.Query).GetValueOrDefault(parameterName, StringValues.Empty);
            if (value.Equals(StringValues.Empty))
                return defaultValue;

            return value;
        }

        public static string GetRelativeUrl(this NavigationManager navigationManager)
        {
            var absoluteUri = new Uri(navigationManager.Uri);
            var baseUri = new Uri(navigationManager.BaseUri);

            return baseUri.MakeRelativeUri(absoluteUri).ToString();
        }
    }
}
