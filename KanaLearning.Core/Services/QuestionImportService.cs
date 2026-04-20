using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using KanaLearning.Models;

namespace KanaLearning.Services;

public sealed class QuestionImportService : IQuestionImportService
{
    private static readonly HashSet<char> HiraganaSmallYoonChars = new() { 'ゃ', 'ゅ', 'ょ' };
    private static readonly HashSet<char> KatakanaSmallYoonChars = new() { 'ャ', 'ュ', 'ョ' };
    private static readonly HashSet<char> DakutenChars = new()
    {
        'が', 'ぎ', 'ぐ', 'げ', 'ご', 'ざ', 'じ', 'ず', 'ぜ', 'ぞ',
        'だ', 'ぢ', 'づ', 'で', 'ど', 'ば', 'び', 'ぶ', 'べ', 'ぼ',
        'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ',
        'ガ', 'ギ', 'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', 'ゾ',
        'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ', 'ブ', 'ベ', 'ボ',
        'パ', 'ピ', 'プ', 'ペ', 'ポ',
    };

    public async Task<ImportResult> ImportFromPathAsync(string path)
    {
        ImportResult result = new();
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            result.Errors.Add("File not found.");
            return result;
        }

        string extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension == ".json")
        {
            return await ImportJsonAsync(path).ConfigureAwait(false);
        }

        if (extension == ".txt")
        {
            return await ImportTxtAsync(path).ConfigureAwait(false);
        }

        result.Errors.Add("Unsupported file type.");
        return result;
    }

    private static async Task<ImportResult> ImportJsonAsync(string path)
    {
        ImportResult result = new();
        try
        {
            string json = await File.ReadAllTextAsync(path).ConfigureAwait(false);
            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true,
            };

            JsonQuestion[]? rawQuestions = JsonSerializer.Deserialize<JsonQuestion[]>(json, options);
            if (rawQuestions is null)
            {
                result.Errors.Add("JSON content is empty.");
                return result;
            }

            int lineNumber = 0;
            foreach (JsonQuestion rawQuestion in rawQuestions)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(rawQuestion.Kana) || string.IsNullOrWhiteSpace(rawQuestion.Romaji))
                {
                    result.Errors.Add($"Invalid JSON record at index {lineNumber}.");
                    continue;
                }

                KanaCategory category = ParseCategory(rawQuestion.Category, rawQuestion.Kana);
                result.Questions.Add(new KanaQuestion
                {
                    Kana = rawQuestion.Kana.Trim(),
                    Romaji = rawQuestion.Romaji.Trim(),
                    Category = category,
                });
            }
        }
        catch (JsonException)
        {
            result.Errors.Add("Invalid JSON format.");
        }
        catch (IOException)
        {
            result.Errors.Add("Unable to read JSON file.");
        }

        return result;
    }

    private static async Task<ImportResult> ImportTxtAsync(string path)
    {
        ImportResult result = new();
        string[] lines;

        try
        {
            lines = await File.ReadAllLinesAsync(path).ConfigureAwait(false);
        }
        catch (IOException)
        {
            result.Errors.Add("Unable to read TXT file.");
            return result;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            string[] parts = ParseTxtLine(line);
            if (parts.Length < 2)
            {
                result.Errors.Add($"Invalid TXT line {i + 1}.");
                continue;
            }

            string kana = parts[0].Trim();
            string romaji = parts[1].Trim();
            if (string.IsNullOrWhiteSpace(kana) || string.IsNullOrWhiteSpace(romaji))
            {
                result.Errors.Add($"Invalid TXT line {i + 1}.");
                continue;
            }

            KanaCategory category = parts.Length >= 3
                ? ParseCategory(parts[2], kana)
                : DetectCategory(kana);

            result.Questions.Add(new KanaQuestion
            {
                Kana = kana,
                Romaji = romaji,
                Category = category,
            });
        }

        return result;
    }

    private static string[] ParseTxtLine(string line)
    {
        int equalsIndex = line.IndexOf('=');
        if (equalsIndex >= 0)
        {
            string left = line[..equalsIndex];
            string right = line[(equalsIndex + 1)..];
            return new[] { left, right };
        }

        return line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    private static KanaCategory ParseCategory(string? rawCategory, string kana)
    {
        if (!string.IsNullOrWhiteSpace(rawCategory) &&
            Enum.TryParse(rawCategory, true, out KanaCategory parsed))
        {
            return parsed;
        }

        return DetectCategory(kana);
    }

    private static KanaCategory DetectCategory(string kana)
    {
        foreach (char character in kana)
        {
            if (HiraganaSmallYoonChars.Contains(character) || KatakanaSmallYoonChars.Contains(character))
            {
                return KanaCategory.Yoon;
            }
        }

        foreach (char character in kana)
        {
            if (DakutenChars.Contains(character))
            {
                return KanaCategory.DakutenHandakuten;
            }
        }

        foreach (char character in kana)
        {
            if (character >= 'ァ' && character <= 'ヿ')
            {
                return KanaCategory.Katakana;
            }
        }

        return KanaCategory.Hiragana;
    }

    private sealed class JsonQuestion
    {
        public string Kana { get; set; } = string.Empty;

        public string Romaji { get; set; } = string.Empty;

        public string? Category { get; set; }
    }
}
