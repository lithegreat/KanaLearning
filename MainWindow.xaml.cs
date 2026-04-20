using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using KanaLearning.Services;
using KanaLearning.ViewModels;
using KanaLearning.Views;

namespace KanaLearning;

public sealed partial class MainWindow : Window
{
    private readonly QuizViewModel _quizViewModel;
    private readonly SettingsViewModel _settingsViewModel;
    private readonly LocalizationService _localizationService;

    public MainWindow(
        QuizViewModel quizViewModel,
        SettingsViewModel settingsViewModel,
        LocalizationService localizationService)
    {
        InitializeComponent();

        _quizViewModel = quizViewModel;
        _settingsViewModel = settingsViewModel;
        _localizationService = localizationService;
        _localizationService.LanguageChanged += OnLanguageChanged;
        Closed += OnWindowClosed;

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");
        UpdateShellTexts();
        NavView.SelectedItem = QuizNavButton;
        ContentFrame.Navigate(typeof(QuizPage), _quizViewModel);
    }

    private void OnNavItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer == QuizNavButton)
        {
            if (ContentFrame.Content is not QuizPage)
            {
                ContentFrame.Navigate(typeof(QuizPage), _quizViewModel);
            }
        }
        else if (args.InvokedItemContainer == SettingsNavButton)
        {
            if (ContentFrame.Content is not SettingsPage)
            {
                ContentFrame.Navigate(typeof(SettingsPage), _settingsViewModel);
            }
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateShellTexts();
    }

    private void UpdateShellTexts()
    {
        AppTitleBar.Title = _localizationService.GetString("Common.AppTitle");
        QuizNavButton.Content = _localizationService.GetString("Nav.Quiz");
        SettingsNavButton.Content = _localizationService.GetString("Nav.Settings");
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        _localizationService.LanguageChanged -= OnLanguageChanged;
        Closed -= OnWindowClosed;
    }
}
