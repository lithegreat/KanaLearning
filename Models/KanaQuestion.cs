namespace KanaLearning.Models;

public sealed class KanaQuestion
{
    public required string Kana { get; init; }

    public required string Romaji { get; init; }

    public KanaCategory Category { get; init; }
}
