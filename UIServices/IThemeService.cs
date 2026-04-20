using System;
using Microsoft.UI.Xaml;

namespace KanaLearning.UIServices;

public interface IThemeService
{
    ElementTheme CurrentTheme { get; }
    void SetTheme(ElementTheme theme);
    void Initialize();
    event EventHandler? ThemeChanged;
}
