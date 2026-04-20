using KanaLearning.Models;

namespace KanaLearning.Services;

public sealed class QuizEvaluationService : IQuizEvaluationService
{
    public bool IsCorrect(KanaQuestion question, string answer)
    {
        return Normalize(question.Romaji) == Normalize(answer);
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}
