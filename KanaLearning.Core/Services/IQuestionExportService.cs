using System.Collections.Generic;
using System.Threading.Tasks;
using KanaLearning.Models;

namespace KanaLearning.Services;

public interface IQuestionExportService
{
    Task ExportToPathAsync(string path, IEnumerable<KanaQuestion> questions);
}
