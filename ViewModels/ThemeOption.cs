using Microsoft.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;

namespace KanaLearning.ViewModels;

public sealed partial class ThemeOption : ObservableObject
{
    public required ElementTheme Theme { get; init; }

    [ObservableProperty]
    private string _displayName = string.Empty;
}
