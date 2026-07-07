using SewQ.Services.Interfaces;
using SewQ.Services.Models;

namespace SewQ.App.State;

/// <summary>
/// UI-side holder for the active theme. Theme and dark mode apply instantly
/// (and persist instantly), matching the prototype's settings sheet behaviour.
/// </summary>
public sealed class ThemeState(ISettingsService settings)
{
    private bool _loaded;

    public SewQTheme Theme { get; private set; } = SewQTheme.Atelier;
    public bool DarkMode { get; private set; }

    public event Action? Changed;

    public string CssClass => Theme switch
    {
        SewQTheme.CraftRoom => "theme-b",
        SewQTheme.Studio => "theme-c",
        _ => "theme-a",
    } + (DarkMode ? " dark" : "");

    public async Task EnsureLoadedAsync()
    {
        if (_loaded)
            return;
        var current = await settings.GetAsync();
        Theme = current.Theme;
        DarkMode = current.DarkMode;
        _loaded = true;
        Changed?.Invoke();
    }

    public async Task SetThemeAsync(SewQTheme theme)
    {
        Theme = theme;
        Changed?.Invoke();
        await PersistAsync();
    }

    public async Task ToggleDarkModeAsync()
    {
        DarkMode = !DarkMode;
        Changed?.Invoke();
        await PersistAsync();
    }

    private async Task PersistAsync()
    {
        var current = await settings.GetAsync();
        await settings.SaveAsync(current with { Theme = Theme, DarkMode = DarkMode });
    }
}
