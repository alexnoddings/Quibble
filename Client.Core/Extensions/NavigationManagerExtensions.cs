using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Quibble.Client.Core.Extensions;

public static class NavigationManagerExtensions
{
	[return: NotNullIfNotNull("defaultValue")]
	public static string? GetQueryParameter(this NavigationManager navigationManager, string parameterName, string? defaultValue = null, bool unEscape = false)
	{
		var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

		var value = QueryHelpers.ParseQuery(uri.Query).GetValueOrDefault(parameterName, StringValues.Empty);
		string? returnValue;
		if (value.Equals(StringValues.Empty))
			returnValue = defaultValue;
		else
			returnValue = value;

		return unEscape ? Uri.UnescapeDataString(returnValue ?? string.Empty) : returnValue;
	}

	public static string GetRelativeUrl(this NavigationManager navigationManager)
	{
		var absoluteUri = new Uri(navigationManager.Uri);
		var baseUri = new Uri(navigationManager.BaseUri);

		return baseUri.MakeRelativeUri(absoluteUri).ToString();
	}
}
