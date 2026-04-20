using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using KanaLearning.Services;
using KanaLearning.UIServices;
using CommunityToolkit.Mvvm.Messaging;

namespace KanaLearning.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ILocalizationService _localizationService;
    private readonly IThemeService _themeService;

    private readonly LanguageOption _systemOption;
    private readonly LanguageOption _zhCnOption;
    private readonly LanguageOption _enUsOption;

    private readonly ThemeOption _themeSystemOption;
    private readonly ThemeOption _themeLightOption;
    private readonly ThemeOption _themeDarkOption;

    private bool _isUpdating;

    public SettingsViewModel(ILocalizationService localizationService, IThemeService themeService)
    {
        _localizationService = localizationService;
        _themeService = themeService;

        _systemOption = new LanguageOption { Code = "system", DisplayName = string.Empty };
        _zhCnOption = new LanguageOption { Code = "zh-CN", DisplayName = string.Empty };
        _enUsOption = new LanguageOption { Code = "en-US", DisplayName = string.Empty };

        _themeSystemOption = new ThemeOption { Theme = Microsoft.UI.Xaml.ElementTheme.Default, DisplayName = string.Empty };
        _themeLightOption = new ThemeOption { Theme = Microsoft.UI.Xaml.ElementTheme.Light, DisplayName = string.Empty };
        _themeDarkOption = new ThemeOption { Theme = Microsoft.UI.Xaml.ElementTheme.Dark, DisplayName = string.Empty };

        LanguageOptions = new ObservableCollection<LanguageOption>
        {
            _systemOption,
            _zhCnOption,
            _enUsOption,
        };

        ThemeOptions = new ObservableCollection<ThemeOption>
        {
            _themeSystemOption,
            _themeLightOption,
            _themeDarkOption,
        };

        RefreshDisplayNames();
        SyncSelectedLanguage();
        SyncSelectedTheme();

        _localizationService.LanguageChanged += OnLanguageChanged;
        _themeService.ThemeChanged += OnThemeChanged;
    }

    public ObservableCollection<LanguageOption> LanguageOptions { get; }
    public ObservableCollection<ThemeOption> ThemeOptions { get; }

    [ObservableProperty]
    private LanguageOption? _selectedLanguage;

    [ObservableProperty]
    private ThemeOption? _selectedTheme;

    public string TitleText => _localizationService.GetString("Settings.Title");

    public string LanguageText => _localizationService.GetString("Settings.Language");

    public string ThemeText => _localizationService.GetString("Settings.Theme");

    public string DescriptionText => _localizationService.GetString("Settings.Description");

    public string UpdateText => _localizationService.GetString("Settings.Update");

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

    partial void OnSelectedThemeChanged(ThemeOption? value)
    {
        if (_isUpdating || value is null)
        {
            return;
        }

        _themeService.SetTheme(value.Theme);
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
            OnPropertyChanged(nameof(ThemeText));
            OnPropertyChanged(nameof(DescriptionText));
            OnPropertyChanged(nameof(UpdateText));
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        _isUpdating = true;
        try
        {
            SyncSelectedTheme();
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

        _themeSystemOption.DisplayName = _localizationService.GetString("Settings.Theme.System");
        _themeLightOption.DisplayName = _localizationService.GetString("Settings.Theme.Light");
        _themeDarkOption.DisplayName = _localizationService.GetString("Settings.Theme.Dark");
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

    private void SyncSelectedTheme()
    {
        Microsoft.UI.Xaml.ElementTheme currentTheme = _themeService.CurrentTheme;

        foreach (ThemeOption option in ThemeOptions)
        {
            if (option.Theme == currentTheme)
            {
                SelectedTheme = option;
                return;
            }
        }

        SelectedTheme = _themeSystemOption;
    }

    [CommunityToolkit.Mvvm.Input.RelayCommand]
    private void CheckForUpdates()
    {
        WeakReferenceMessenger.Default.Send(new KanaLearning.Messages.CheckForUpdateMessage());
    }
}

