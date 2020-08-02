using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web;
using Microsoft.AspNetCore.Components;

namespace Quibble.Client.Extensions.AspCore
{
    public static class NavigationManagerExtensions
    {
        public static string? GetParameter(this NavigationManager navigationManager, string name)
        {
            if (navigationManager == null) throw new ArgumentNullException(nameof(navigationManager));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Parameter name cannot be empty", nameof(name));

            string currentUriStr = navigationManager.Uri;
            Uri currentUri = new Uri(currentUriStr);
            NameValueCollection query = HttpUtility.ParseQueryString(currentUri.Query);
            return query.Get(name);
        }

        [return: MaybeNull]
        public static T GetParameter<T>(this NavigationManager navigationManager, string name)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter == null || !converter.CanConvertFrom(typeof(string)))
                throw new NotSupportedException($"Cannot convert from string to {typeof(T).Name}");

            string? valueStr = GetParameter(navigationManager, name);
            T value = default;
            try
            {
                value = (T) converter.ConvertFromInvariantString(valueStr);
            }
            catch (FormatException) { }
            catch (ArgumentException) { }

            return value;
        }

        public static void SetParameter(this NavigationManager navigationManager, string name, string? value)
            => SetParameterCore(navigationManager, name, value);

        public static void SetParameter(this NavigationManager navigationManager, string name, int value)
            => SetParameterCore(navigationManager, name, value.ToString(CultureInfo.InvariantCulture));

        public static void SetParameter(this NavigationManager navigationManager, string name, object? value)
            => SetParameterCore(navigationManager, name, value?.ToString());

        private static void SetParameterCore(this NavigationManager navigationManager, string name, string? value)
        {
            if (navigationManager == null) throw new ArgumentNullException(nameof(navigationManager));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Parameter name cannot be empty", nameof(name));

            string currentUriStr = navigationManager.Uri;
            Uri currentUri = new Uri(currentUriStr);
            NameValueCollection query = HttpUtility.ParseQueryString(currentUri.Query);
            query.Set(name, value);

            var newUriBuilder = new UriBuilder(currentUri)
            {
                Query = query.ToString()
            };

            string newUri = newUriBuilder.Uri.ToString();
            navigationManager.NavigateTo(newUri);
        }
    }
}
