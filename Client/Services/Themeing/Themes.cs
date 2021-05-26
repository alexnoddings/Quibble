using Blazorise;

namespace Quibble.Client.Services.Themeing
{
    public class Themes
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
            ButtonOptions = new ThemeButtonOptions
            {
                SmallBorderRadius = RadiusSmall,
                BorderRadius = RadiusMedium,
                LargeBorderRadius = RadiusLarge
            },
            CardOptions = new ThemeCardOptions
            {
                BorderRadius = RadiusMedium
            },
            DividerOptions = new ThemeDividerOptions
            {
                Thickness = "1px"
            },
            InputOptions = new ThemeInputOptions
            {
                BorderRadius = RadiusMedium
            },
            ModalOptions = new ThemeModalOptions
            {
                BorderRadius = RadiusLarge
            }
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
            ButtonOptions = new ThemeButtonOptions
            {
                SmallBorderRadius = RadiusSmall,
                BorderRadius = RadiusMedium,
                LargeBorderRadius = RadiusLarge
            },
            CardOptions = new ThemeCardOptions
            {
                BorderRadius = RadiusMedium
            },
            DividerOptions = new ThemeDividerOptions
            {
                Thickness = "1px"
            },
            InputOptions = new ThemeInputOptions
            {
                BorderRadius = RadiusMedium
            },
            ModalOptions = new ThemeModalOptions
            {
                BorderRadius = RadiusLarge
            }
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
            ButtonOptions = new ThemeButtonOptions
            {
                SmallBorderRadius = RadiusSmall,
                BorderRadius = RadiusMedium,
                LargeBorderRadius = RadiusLarge
            },
            CardOptions = new ThemeCardOptions
            {
                BorderRadius = RadiusMedium
            },
            DividerOptions = new ThemeDividerOptions
            {
                Thickness = "1px"
            },
            InputOptions = new ThemeInputOptions
            {
                BorderRadius = RadiusMedium
            },
            ModalOptions = new ThemeModalOptions
            {
                BorderRadius = RadiusLarge
            }
        };
    }
}
