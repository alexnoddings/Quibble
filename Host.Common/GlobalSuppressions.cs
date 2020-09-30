using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Setters are required for POCOs when using other libraries.", Scope = "namespaceanddescendants", Target = "~N:Quibble.Host.Common.Data.Entities")]

[assembly: SuppressMessage("Critical Code Smell", "S927:parameter names should match base declaration and other partial definitions", Justification = "Parameter names in repository implementations specify the entity type.", Scope = "namespaceanddescendants", Target = "~N:Quibble.Host.Common.Repositories")]
[assembly: SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "These classes are instantiated by the DI container.", Scope = "namespaceanddescendants", Target = "~N:Quibble.Host.Common.Repositories")]

[assembly: SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "These classes are instantiated by the DI container.", Scope = "namespaceanddescendants", Target = "~N:Quibble.Host.Common.Services")]
