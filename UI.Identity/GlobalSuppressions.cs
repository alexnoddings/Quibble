using System.Diagnostics.CodeAnalysis;
using Quibble.UI.Identity;

[assembly: SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "This is the standard for Identity pages.", Scope = "namespaceanddescendants", Target = SuppressionConstants.IdentityNamespace)]
[assembly: SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "This is the standard for Identity pages.", Scope = "namespaceanddescendants", Target = SuppressionConstants.IdentityNamespace)]
[assembly: SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "This is the standard for Identity pages.", Scope = "namespaceanddescendants", Target = SuppressionConstants.IdentityNamespace)]

[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "This is the standard for Identity pages.", Scope = "namespaceanddescendants", Target = SuppressionConstants.IdentityNamespace)]

[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Empty methods are standard in Identity pages, and shouldn't be static.", Scope = "namespaceanddescendants", Target = SuppressionConstants.IdentityNamespace)]
[assembly: SuppressMessage("Critical Code Smell", "S1186:Methods should not be empty", Justification = "Empty methods are standard in Identity pages.", Scope = "namespaceanddescendants", Target = SuppressionConstants.IdentityNamespace)]

namespace Quibble.UI.Identity
{
    internal static class SuppressionConstants
    {
        internal const string IdentityNamespace = "~N:Quibble.UI.Identity.Areas.Identity.Pages.Account";
    }
}