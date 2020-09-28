using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "This is counter-productive in Blazor pages.", Scope = "namespaceanddescendants", Target = "~N:Quibble.UI.Components")]
[assembly: SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "This is counter-productive in Blazor pages.", Scope = "namespaceanddescendants", Target = "~N:Quibble.UI.Pages")]

[assembly: SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped", Justification = "This makes code much more tedious to read for simple null checks in initialisers.", Scope = "namespaceanddescendants", Target = "~N:Quibble.UI.Components")]
[assembly: SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped", Justification = "This makes code much more tedious to read for simple null checks in initialisers.", Scope = "namespaceanddescendants", Target = "~N:Quibble.UI.Pages")]
