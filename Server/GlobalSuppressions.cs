using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "Uri parameters should be strings for Identity pages.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Areas.Identity")]
[assembly: SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Uri properties should be strings for Identity pages.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Areas.Identity")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not necessary for migrations.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Data.Migrations")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Arguments are handled by GRPC and are never null.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Grpc")]

[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Empty members are useful for some Identity pages.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Areas.Identity")]

[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This check generates multiple false-positives for Identity pages.", Scope = "namespaceanddescendants", Target = "Quibble.Server.Areas.Identity")]

[assembly: SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "It is standard for Identity pages to use \"OnPost\" and \"OnGet\".", Scope = "namespaceanddescendants", Target = "Quibble.Server.Areas.Identity")]