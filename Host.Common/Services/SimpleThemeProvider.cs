using Quibble.UI.Core.Services.Theme;

namespace Quibble.Host.Common.Services
{
    internal class SimpleThemeProvider : IThemeProvider
    {
        public IColourSet Primary { get; } = new SimpleThemeSet("#FFFFFF", "#FF5CE8");
        public IColourSet Secondary { get; } = new SimpleThemeSet("#FFFFFF", "#FF5C98");
        public IColourSet Success { get; } = new SimpleThemeSet("#FFFFFF", "#21A831");
        public IColourSet Info { get; } = new SimpleThemeSet("#FFFFFF", "#5CE9FF");
        public IColourSet Warning { get; } = new SimpleThemeSet("#FFFFFF", "#FFB85C");
        public IColourSet Danger { get; } = new SimpleThemeSet("#FFFFFF", "#FF725C");
        public IColourSet Light { get; } = new SimpleThemeSet("#212529", "#F8F9FA"); // Bootstrap default
        public IColourSet Dark { get; } = new SimpleThemeSet("#FFFFFF", "#343A40"); // Bootstrap default

        public string Body { get; } = "#FFFFFF";
        public string Muted { get; } = "#828A93";
        public string White { get; } = "#FAFAFA";
        public string Black50 { get; } = "#000000";
        public string White50 { get; } = "#FFFFFF";

        public byte LuminanceThreshold { get; } = 200;
    }
}
