using Blazorise;

namespace Quibble.Client.Services.Themeing
{
    public static class Themes
    {
        public const string Primary = "#9966CC";
        public const string Secondary = "#28232D";
        public const string Success = "#499922";
        public const string Info = "#5CCCCC";
        public const string Danger = "#CC4433";
        public const string Warning = "#FF7733";
        public const string Light = "#FBFBFB";
        public const string Dark = "#28232D";

        public const string White = "#FFFFFF";
        public const string White50 = "#FFFFFF80";
        public const string Black = "#252A2F";
        public const string Black50 = "#252A2F80";

        public const string RadiusSmall = "4px";
        public const string RadiusMedium = "6px";
        public const string RadiusLarge = "8px";

        private static ThemeButtonOptions DefaultButtonOptions { get; } = new()
        {
            SmallBorderRadius = RadiusSmall,
            BorderRadius = RadiusMedium,
            LargeBorderRadius = RadiusLarge
        };

        private static ThemeCardOptions DefaultCardOptions { get; } = new()
        {
            BorderRadius = RadiusMedium
        };

        private static ThemeDividerOptions DefaultDividerOptions { get; } = new()
        {
            Thickness = "1px"
        };

        private static ThemeInputOptions DefaultInputOptions { get; } = new()
        {
            BorderRadius = RadiusMedium
        };

        private static ThemeModalOptions DefaultModalOptions { get; } = new()
        {
            BorderRadius = RadiusLarge
        };

        private static ThemeContainerMaxWidthOptions DefaultContainerMaxWidthOptions { get; } = new()
        {
            Mobile = "720px",
            Tablet = "960px",
            Desktop = "1140px",
            Widescreen = "1320px",
            FullHD = "1520px"
        };

        private static ThemeBreakpointOptions DefaultBreakpointOptions { get; } = new()
        {
            Mobile = "768px",
            Tablet = "992px",
            Desktop = "1200px",
            Widescreen = "1400px",
            FullHD = "1600px",
        };

        public static Theme LightTheme { get; } = new()
        {
            White = White,
            Black = Black,
            ColorOptions = new ThemeColorOptions
            {
                Primary = Primary,
                Secondary = Secondary,
                Success = Success,
                Info = Info,
                Danger = Danger,
                Warning = Warning,
                Light = Light,
                Dark = Dark
            },
            BackgroundOptions = new ThemeBackgroundOptions
            {
                Primary = Primary,
                Secondary = Secondary,
                Success = Success,
                Info = Info,
                Danger = Danger,
                Warning = Warning,
                Light = Light,
                Dark = Dark,
                Body = White
            },
            TextColorOptions = new ThemeTextColorOptions
            {
                Primary = Primary,
                Secondary = Secondary,
                Success = Success,
                Info = Info,
                Danger = Danger,
                Warning = Warning,
                Light = Light,
                Dark = Dark,
                Body = Black,
                White = White,
                Black50 = Black50,
                White50 = White50,
                Muted = "#777777"
            },
            BreakpointOptions = DefaultBreakpointOptions,
            ButtonOptions = DefaultButtonOptions,
            CardOptions = DefaultCardOptions,
            ContainerMaxWidthOptions = DefaultContainerMaxWidthOptions,
            DividerOptions = DefaultDividerOptions,
            InputOptions = DefaultInputOptions,
            ModalOptions = DefaultModalOptions,
        };

        public static Theme DarkTheme { get; } = new()
        {
            White = Black,
            Black = White,
            ColorOptions = new ThemeColorOptions
            {
                Primary = Primary,
                Secondary = Secondary,
                Success = Success,
                Info = Info,
                Danger = Danger,
                Warning = Warning,
                Light = Dark,
                Dark = Light
            },
            BackgroundOptions = new ThemeBackgroundOptions
            {
                Primary = Primary,
                Secondary = Secondary,
                Success = Success,
                Info = Info,
                Danger = Danger,
                Warning = Warning,
                Light = Secondary,
                Dark = Light,
                Body = Dark
            },
            TextColorOptions = new ThemeTextColorOptions
            {
                Primary = Primary,
                Secondary = Secondary,
                Success = Success,
                Info = Info,
                Danger = Danger,
                Warning = Warning,
                Light = Dark,
                Dark = Light,
                Body = White,
                White = White,
                Black50 = Black50,
                White50 = White50,
                Muted = "#AAAAAA"
            },
            BreakpointOptions = DefaultBreakpointOptions,
            ButtonOptions = DefaultButtonOptions,
            CardOptions = DefaultCardOptions,
            ContainerMaxWidthOptions = DefaultContainerMaxWidthOptions,
            DividerOptions = DefaultDividerOptions,
            InputOptions = DefaultInputOptions,
            ModalOptions = DefaultModalOptions,
        };

        public static Theme ContrastTheme { get; } = new()
        {
            White = "#FFFFFF",
            Black = "#000000",
            ColorOptions = new ThemeColorOptions
            {
                Primary = "#6633AA",
                Secondary = Secondary,
                Success = "#00BB00",
                Info = "#3344CC",
                Danger = "#FF0000",
                Warning = "#FF8800",
                Light = Light,
                Dark = Secondary
            },
            BackgroundOptions = new ThemeBackgroundOptions
            {
                Primary = "#6633AA",
                Secondary = Secondary,
                Success = "#00BB00",
                Info = "#3344CC",
                Danger = "#FF0000",
                Warning = "#FF8800",
                Light = Light,
                Dark = Secondary,
                Body = "#FFFFFF"
            },
            TextColorOptions = new ThemeTextColorOptions
            {
                Primary = "#6633AA",
                Secondary = Secondary,
                Success = "#00BB00",
                Info = "#3344CC",
                Danger = "#FF0000",
                Warning = "#FF8800",
                Light = Light,
                Dark = Secondary,
                Body = "#000000",
                White = "#FFFFFF",
                Black50 = "#000000",
                White50 = "#FFFFFF"
            },
            BreakpointOptions = DefaultBreakpointOptions,
            ButtonOptions = DefaultButtonOptions,
            CardOptions = DefaultCardOptions,
            ContainerMaxWidthOptions = DefaultContainerMaxWidthOptions,
            DividerOptions = DefaultDividerOptions,
            InputOptions = DefaultInputOptions,
            ModalOptions = DefaultModalOptions,
        };
    }
}
