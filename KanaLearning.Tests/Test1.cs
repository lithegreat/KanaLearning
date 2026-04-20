using KanaLearning.Models;
using KanaLearning.Services;

namespace KanaLearning.Tests;

[TestClass]
public sealed class QuizEvaluationServiceTests
{
    [TestMethod]
    public void IsCorrect_ExactRomaji_ReturnsTrue()
    {
        QuizEvaluationService service = new();
        KanaQuestion question = new()
        {
            Kana = "きゃ",
            Romaji = "kya",
            Category = KanaCategory.Yoon,
        };

        bool actual = service.IsCorrect(question, "kya");

        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void IsCorrect_CaseAndTrimVariant_ReturnsTrue()
    {
        QuizEvaluationService service = new();
        KanaQuestion question = new()
        {
            Kana = "が",
            Romaji = "ga",
            Category = KanaCategory.DakutenHandakuten,
        };

        bool actual = service.IsCorrect(question, "  GA  ");

        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void IsCorrect_WrongRomaji_ReturnsFalse()
    {
        QuizEvaluationService service = new();
        KanaQuestion question = new()
        {
            Kana = "し",
            Romaji = "shi",
            Category = KanaCategory.Hiragana,
        };

        bool actual = service.IsCorrect(question, "si");

        Assert.IsFalse(actual);
    }
}
