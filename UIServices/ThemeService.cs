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

    private string GetSettingsFilePath()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appFolder = System.IO.Path.Combine(localAppData, "KanaLearning");
        System.IO.Directory.CreateDirectory(appFolder);
        return System.IO.Path.Combine(appFolder, "theme.txt");
    }

    private ElementTheme LoadThemeFromSettings()
    {
        try
        {
            string filePath = GetSettingsFilePath();
            if (System.IO.File.Exists(filePath))
            {
                string themeString = System.IO.File.ReadAllText(filePath);
                if (Enum.TryParse(themeString, out ElementTheme theme))
                {
                    return theme;
                }
            }
        }
        catch
        {
            // Ignore errors
        }
        return ElementTheme.Default;
    }

    private void SaveThemeToSettings(ElementTheme theme)
    {
        try
        {
            string filePath = GetSettingsFilePath();
            System.IO.File.WriteAllText(filePath, theme.ToString());
        }
        catch
        {
            // Ignore errors
        }
    }
}
