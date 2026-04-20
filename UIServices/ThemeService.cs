using System;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace KanaLearning.UIServices;

public sealed class ThemeService : IThemeService
{
    private const string ThemeKey = "AppTheme";

    public ThemeService()
    {
        CurrentTheme = LoadThemeFromSettings();
    }

    public ElementTheme CurrentTheme { get; private set; }

    public event EventHandler? ThemeChanged;

    public void SetTheme(ElementTheme theme)
    {
        if (CurrentTheme == theme)
        {
            return;
        }

        CurrentTheme = theme;
        SaveThemeToSettings(theme);
        ApplyTheme(theme);
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Initialize()
    {
        ApplyTheme(CurrentTheme);
    }

    private void ApplyTheme(ElementTheme theme)
    {
        if (App.CurrentApp.MainWindow?.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = theme;
        }
    }

    private ElementTheme LoadThemeFromSettings()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(ThemeKey, out object? value) && value is string themeString)
        {
            if (Enum.TryParse(themeString, out ElementTheme theme))
            {
                return theme;
            }
        }
        return ElementTheme.Default;
    }

    private void SaveThemeToSettings(ElementTheme theme)
    {
        ApplicationData.Current.LocalSettings.Values[ThemeKey] = theme.ToString();
    }
}
