using System.Collections.Generic;

namespace KanaLearning.Models;

public sealed class ImportResult
{
    public List<KanaQuestion> Questions { get; } = new();

    public List<string> Errors { get; } = new();
}
