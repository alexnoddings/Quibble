namespace Quibble.UI.Core.Services.Theme
{
    public class SimpleThemeProvider : IThemeProvider
    {
        public IColourSet Primary { get; } = new SimpleColourSet("#FFFFFF", "#FF5CE8");
        public IColourSet Secondary { get; } = new SimpleColourSet("#FFFFFF", "#FF5C98");
        public IColourSet Success { get; } = new SimpleColourSet("#FFFFFF", "#21A831");
        public IColourSet Info { get; } = new SimpleColourSet("#FFFFFF", "#44AAAA");
        public IColourSet Warning { get; } = new SimpleColourSet("#FFFFFF", "#FFB85C");
        public IColourSet Danger { get; } = new SimpleColourSet("#FFFFFF", "#FF725C");
        public IColourSet Light { get; } = new SimpleColourSet("#212529", "#F8F9FA"); // Bootstrap default
        public IColourSet Dark { get; } = new SimpleColourSet("#FFFFFF", "#343A40"); // Bootstrap default

        public string Body { get; } = "#FFFFFF";
        public string Muted { get; } = "#828A93";
        public string White { get; } = "#FAFAFA";
        public string Black50 { get; } = "#000000";
        public string White50 { get; } = "#FFFFFF";

        public byte LuminanceThreshold { get; } = 200;
    }
}
