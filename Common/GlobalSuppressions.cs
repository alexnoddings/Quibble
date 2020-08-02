using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Setters must be present for data to be de/serialised properly.", Scope = "namespaceanddescendants", Target = "Quibble.Common")]

[assembly: SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Temporarily suppressed during development.", Scope = "namespaceanddescendants", Target = "Quibble.Common")]