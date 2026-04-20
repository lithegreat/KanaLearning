using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using KanaLearning.Services;

namespace KanaLearning.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ILocalizationService _localizationService;

    public SettingsViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        _localizationService.LanguageChanged += OnLanguageChanged;

        LanguageOptions = new ObservableCollection<LanguageOption>();
        RebuildLanguageOptions();
    }

    public ObservableCollection<LanguageOption> LanguageOptions { get; }

    [ObservableProperty]
    private LanguageOption? _selectedLanguage;

    public string TitleText => _localizationService.GetString("Settings.Title");

    public string LanguageText => _localizationService.GetString("Settings.Language");

    public string DescriptionText => _localizationService.GetString("Settings.Description");

    partial void OnSelectedLanguageChanged(LanguageOption? value)
    {
        if (value is null)
        {
            return;
        }

        if (value.Code == "system")
        {
            _localizationService.UseSystemLanguage();
            return;
        }

        _localizationService.SetLanguage(value.Code);
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        RebuildLanguageOptions();
        OnPropertyChanged(string.Empty);
    }

    private void RebuildLanguageOptions()
    {
        string currentCode = _localizationService.CurrentLanguageCode;

        LanguageOptions.Clear();
        LanguageOptions.Add(new LanguageOption
        {
            Code = "system",
            DisplayName = _localizationService.GetString("Settings.Language.System"),
        });
        LanguageOptions.Add(new LanguageOption
        {
            Code = "zh-CN",
            DisplayName = _localizationService.GetString("Settings.Language.ZhCn"),
        });
        LanguageOptions.Add(new LanguageOption
        {
            Code = "en-US",
            DisplayName = _localizationService.GetString("Settings.Language.EnUs"),
        });

        foreach (LanguageOption option in LanguageOptions)
        {
            if (option.Code == currentCode)
            {
                SelectedLanguage = option;
                return;
            }
        }

        SelectedLanguage = LanguageOptions[0];
    }
}
