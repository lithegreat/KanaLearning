using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using KanaLearning.Services;

namespace KanaLearning.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ILocalizationService _localizationService;
    private readonly LanguageOption _systemOption;
    private readonly LanguageOption _zhCnOption;
    private readonly LanguageOption _enUsOption;
    private bool _isUpdating;

    public SettingsViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;

        _systemOption = new LanguageOption { Code = "system", DisplayName = string.Empty };
        _zhCnOption = new LanguageOption { Code = "zh-CN", DisplayName = string.Empty };
        _enUsOption = new LanguageOption { Code = "en-US", DisplayName = string.Empty };

        LanguageOptions = new ObservableCollection<LanguageOption>
        {
            _systemOption,
            _zhCnOption,
            _enUsOption,
        };

        RefreshDisplayNames();
        SyncSelectedLanguage();

        _localizationService.LanguageChanged += OnLanguageChanged;
    }

    public ObservableCollection<LanguageOption> LanguageOptions { get; }

    [ObservableProperty]
    private LanguageOption? _selectedLanguage;

    public string TitleText => _localizationService.GetString("Settings.Title");

    public string LanguageText => _localizationService.GetString("Settings.Language");

    public string DescriptionText => _localizationService.GetString("Settings.Description");

    partial void OnSelectedLanguageChanged(LanguageOption? value)
    {
        if (_isUpdating || value is null)
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
        _isUpdating = true;
        try
        {
            RefreshDisplayNames();
            SyncSelectedLanguage();

            OnPropertyChanged(nameof(TitleText));
            OnPropertyChanged(nameof(LanguageText));
            OnPropertyChanged(nameof(DescriptionText));
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private void RefreshDisplayNames()
    {
        _systemOption.DisplayName = _localizationService.GetString("Settings.Language.System");
        _zhCnOption.DisplayName = _localizationService.GetString("Settings.Language.ZhCn");
        _enUsOption.DisplayName = _localizationService.GetString("Settings.Language.EnUs");
    }

    private void SyncSelectedLanguage()
    {
        string currentCode = _localizationService.CurrentLanguageCode;

        foreach (LanguageOption option in LanguageOptions)
        {
            if (option.Code == currentCode)
            {
                SelectedLanguage = option;
                return;
            }
        }

        SelectedLanguage = _systemOption;
    }
}

