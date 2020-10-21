namespace Quibble.UI.Core.Services.Theme
{
    public record SimpleColourSet : IColourSet
    {
        public string Text { get; init; }
        public string Colour { get; init; }

        public SimpleColourSet(string text, string colour)
            => (Text, Colour) = (text, colour);

        public void Deconstruct(out string text, out string colour)
            => (text, colour) = (Text, Colour);
    }
}
