using System.Threading.Tasks;
using KanaLearning.Models;

namespace KanaLearning.Services;

public interface IQuestionImportService
{
    Task<ImportResult> ImportFromPathAsync(string path);
}
