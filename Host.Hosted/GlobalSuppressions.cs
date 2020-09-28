using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "MigrationBuilders will never be null.", Scope = "namespaceanddescendants", Target = "~N:Quibble.Host.Hosted.Data.Migrations")]

[assembly: SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "These classes are instantiated by the DI container.", Scope = "namespaceanddescendants", Target = "~N:Quibble.Host.Hosted.Platform.Events")] 
