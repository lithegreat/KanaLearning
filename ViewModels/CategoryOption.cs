using KanaLearning.Models;

namespace KanaLearning.ViewModels;

public sealed class CategoryOption
{
    public required KanaCategory Category { get; init; }

    public required string DisplayName { get; init; }
}
