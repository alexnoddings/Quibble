namespace Quibble.UI.Core.Services.Theme
{
    public interface IThemeProvider
    {
        public IColourSet Primary { get; }
        public IColourSet Secondary { get; }
        public IColourSet Success { get; }
        public IColourSet Info { get; }
        public IColourSet Warning { get; }
        public IColourSet Danger { get; }
        public IColourSet Light { get; }
        public IColourSet Dark { get; }

        public string Body { get; }
        public string Muted { get; }
        public string White { get; }
        public string Black50 { get; }
        public string White50 { get; }

        public byte LuminanceThreshold { get; }
    }
}
