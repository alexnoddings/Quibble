using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "Uri parameters should be strings for Identity pages.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Areas.Identity")]
[assembly: SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Uri properties should be strings for Identity pages.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Areas.Identity")]

[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Empty members are useful for some Identity pages.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Areas.Identity")]

[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This check generates multiple false-positives for Identity pages.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Areas.Identity")]
