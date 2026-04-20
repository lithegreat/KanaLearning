using KanaLearning.Models;
using KanaLearning.Services;

namespace KanaLearning.Tests;

[TestClass]
public sealed class QuestionImportServiceTests
{
    [TestMethod]
    public async Task ImportFromPathAsync_TxtFile_ParsesQuestionsAndCategories()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        string content = string.Join(
            Environment.NewLine,
            "あ=a",
            "ガ=ga",
            "きゃ=kya");

        try
        {
            await File.WriteAllTextAsync(path, content);
            QuestionImportService service = new();

            ImportResult result = await service.ImportFromPathAsync(path);

            Assert.AreEqual(3, result.Questions.Count);
            Assert.AreEqual(KanaCategory.Hiragana, result.Questions[0].Category);
            Assert.AreEqual(KanaCategory.DakutenHandakuten, result.Questions[1].Category);
            Assert.AreEqual(KanaCategory.Yoon, result.Questions[2].Category);
            Assert.AreEqual(0, result.Errors.Count);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [TestMethod]
    public async Task ImportFromPathAsync_JsonFile_ParsesQuestions()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        string json = """
        [
          { "kana": "カ", "romaji": "ka", "category": "Katakana" },
          { "kana": "ぴ", "romaji": "pi", "category": "DakutenHandakuten" }
        ]
        """;

        try
        {
            await File.WriteAllTextAsync(path, json);
            QuestionImportService service = new();

            ImportResult result = await service.ImportFromPathAsync(path);

            Assert.AreEqual(2, result.Questions.Count);
            Assert.AreEqual(KanaCategory.Katakana, result.Questions[0].Category);
            Assert.AreEqual("pi", result.Questions[1].Romaji);
            Assert.AreEqual(0, result.Errors.Count);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [TestMethod]
    public async Task ImportFromPathAsync_UnsupportedExtension_ReturnsError()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csv");

        try
        {
            await File.WriteAllTextAsync(path, "あ,a");
            QuestionImportService service = new();

            ImportResult result = await service.ImportFromPathAsync(path);

            Assert.AreEqual(0, result.Questions.Count);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("Unsupported file type.", result.Errors[0]);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
