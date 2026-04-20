using CommunityToolkit.Mvvm.ComponentModel;

namespace KanaLearning.ViewModels;

public sealed partial class LanguageOption : ObservableObject
{
    public required string Code { get; init; }

    [ObservableProperty]
    public partial string DisplayName { get; set; } = string.Empty;
}
