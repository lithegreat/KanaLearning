using System;
using System.Collections.Generic;

namespace KanaLearning.Services;

public interface ILocalizationService
{
    event EventHandler? LanguageChanged;

    string CurrentLanguageCode { get; }

    IReadOnlyList<string> SupportedLanguages { get; }

    string GetString(string key);

    void SetLanguage(string languageCode);

    void UseSystemLanguage();
}
