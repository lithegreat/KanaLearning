using KanaLearning.Models;

namespace KanaLearning.Services;

public interface IQuizEvaluationService
{
    bool IsCorrect(KanaQuestion question, string answer);
}
