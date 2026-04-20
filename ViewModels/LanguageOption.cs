using CommunityToolkit.Mvvm.ComponentModel;

namespace KanaLearning.ViewModels;

public sealed partial class LanguageOption : ObservableObject
{
    public required string Code { get; init; }

    [ObservableProperty]
    private string _displayName = string.Empty;
}
