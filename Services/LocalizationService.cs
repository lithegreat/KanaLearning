using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace KanaLearning.Services;

public sealed class LocalizationService : ILocalizationService
{
    private readonly Dictionary<string, string> _resources = new();

    public LocalizationService()
    {
        SupportedLanguages = new List<string> { "en-US", "zh-CN" };
        CurrentLanguageCode = ResolveSystemLanguage();
        LoadResources(CurrentLanguageCode);
    }

    public event EventHandler? LanguageChanged;

    public string CurrentLanguageCode { get; private set; }

    public IReadOnlyList<string> SupportedLanguages { get; }

    public string GetString(string key)
    {
        if (_resources.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return key;
    }

    public void SetLanguage(string languageCode)
    {
        if (!SupportedLanguages.Any(code => string.Equals(code, languageCode, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        if (string.Equals(CurrentLanguageCode, languageCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        CurrentLanguageCode = languageCode;
        LoadResources(CurrentLanguageCode);
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UseSystemLanguage()
    {
        string systemLanguage = ResolveSystemLanguage();
        if (string.Equals(CurrentLanguageCode, systemLanguage, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        CurrentLanguageCode = systemLanguage;
        LoadResources(CurrentLanguageCode);
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    private void LoadResources(string languageCode)
    {
        _resources.Clear();

        string fallbackPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Localization", "en-US.json");
        LoadFromFile(fallbackPath);

        string languagePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Localization", languageCode + ".json");
        if (!string.Equals(languagePath, fallbackPath, StringComparison.OrdinalIgnoreCase))
        {
            LoadFromFile(languagePath);
        }
    }

    private void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        string json = File.ReadAllText(filePath);
        Dictionary<string, string>? values = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (values is null)
        {
            return;
        }

        foreach (KeyValuePair<string, string> pair in values)
        {
            _resources[pair.Key] = pair.Value;
        }
    }

    private static string ResolveSystemLanguage()
    {
        string culture = CultureInfo.CurrentUICulture.Name;
        if (culture.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
        {
            return "zh-CN";
        }

        return "en-US";
    }
}
