using Quibble.UI.Core.Services.Theme;

namespace Quibble.Host.Common.Services
{
    public record SimpleThemeSet : IColourSet
    {
        public string Text { get; init; }
        public string Colour { get; init; }

        public SimpleThemeSet(string text, string colour)
            => (Text, Colour) = (text, colour);

        public void Deconstruct(out string text, out string colour)
            => (text, colour) = (Text, Colour);
    }
}
