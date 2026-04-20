using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using KanaLearning.Models;

namespace KanaLearning.Services;

public sealed class QuestionExportService : IQuestionExportService
{
    public async Task ExportToPathAsync(string path, IEnumerable<KanaQuestion> questions)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        string json = JsonSerializer.Serialize(questions, options);
        await File.WriteAllTextAsync(path, json).ConfigureAwait(false);
    }
}
