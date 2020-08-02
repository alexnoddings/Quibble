using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Component parameters (e.g. attributes) require public setters.", Scope = "namespaceanddescendants", Target = "Quibble.Client.Components")]

namespace Quibble.Client
{
    /// <summary>
    /// Justifications for commonly suppressed messages.
    /// </summary>
    internal static class SuppressionJustifications
    {
        /// <summary>
        /// While Uris should be exposed as <see cref="System.Uri"/>, it makes using them significantly more tedious.
        /// For the purposes of this project, there is very little to be gained for the usage overhead.
        /// </summary>
        public const string CA1054 =
            "Uri parameters are exposed as strings for ease of use. They are converted to Uris elsewhere for ease of use.";
    }
}