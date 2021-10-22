using Blazored.LocalStorage;
using Blazorise;

namespace Quibble.Client.Services.Themeing;

public class ThemeService
{
    private const string LocalStorageKey = "q-theme";

    public Theme Theme { get; private set; } = Themes.LightTheme;
    public UserTheme UserTheme { get; private set; } = UserTheme.Light;

    public event Func<Task>? ThemeUpdated;

    private ILocalStorageService LocalStorageService { get; }

    public ThemeService(ILocalStorageService localStorageService)
    {
        this.LocalStorageService = localStorageService;
    }

    public async Task InitialiseAsync()
    {
        var userTheme = await LocalStorageService.GetItemAsync<UserTheme?>(LocalStorageKey);
        if (userTheme is null)
        {
            userTheme = UserTheme.Light;
            await LocalStorageService.SetItemAsync(LocalStorageKey, userTheme);
        }

        UserTheme = userTheme.Value;
        SetTheme(UserTheme);
    }

    public async Task UpdateAsync(UserTheme newUserTheme)
    {
        await LocalStorageService.SetItemAsync(LocalStorageKey, newUserTheme);
        SetTheme(newUserTheme);

        if (ThemeUpdated is not null)
            await ThemeUpdated.Invoke();
    }

    private void SetTheme(UserTheme newUserTheme)
    {
        UserTheme = newUserTheme;

        Theme = UserTheme switch
        {
            UserTheme.Light => Themes.LightTheme,
            UserTheme.Dark => Themes.DarkTheme,
            UserTheme.Contrast => Themes.ContrastTheme,
            _ => throw new ArgumentException("Invalid theme.", nameof(newUserTheme))
        };
    }
}
