using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using AutoUpdaterDotNET;
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
        
        CheckForUpdatesAsync();
    }

    private async void CheckForUpdatesAsync()
    {
        try
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("KanaLearning", "1.0"));
            string apiUrl = "https://api.github.com/repos/lithegreat/KanaLearning/releases/latest";
            
            string jsonResponse = await client.GetStringAsync(apiUrl).ConfigureAwait(true);
            
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            string tagName = root.GetProperty("tag_name").GetString() ?? "0.0.0";
            string versionStr = tagName.TrimStart('v', 'V');
            
            string changelogUrl = root.GetProperty("html_url").GetString() ?? string.Empty;
            string downloadUrl = string.Empty;
            
            if (root.TryGetProperty("assets", out JsonElement assets) && assets.GetArrayLength() > 0)
            {
                downloadUrl = assets[0].GetProperty("browser_download_url").GetString() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(downloadUrl)) return;

            string tempFilePath = Path.Combine(Path.GetTempPath(), "KanaLearningUpdate.xml");
            await File.WriteAllTextAsync(tempFilePath, "<dummy/>");

            DispatcherQueue.TryEnqueue(() =>
            {
                AutoUpdater.ParseUpdateInfoEvent += (args) =>
                {
                    args.UpdateInfo = new UpdateInfoEventArgs
                    {
                        CurrentVersion = versionStr,
                        ChangelogURL = changelogUrl,
                        DownloadURL = downloadUrl,
                        Mandatory = new Mandatory
                        {
                            Value = false,
                            UpdateMode = Mode.Normal,
                            MinimumVersion = "0.0.0.0"
                        }
                    };
                };

                AutoUpdater.CheckForUpdateEvent += async (args) =>
                {
                    if (args.Error == null && args.IsUpdateAvailable)
                    {
                        var dialog = new ContentDialog
                        {
                            Title = "Update Available",
                            Content = $"A new version ({args.CurrentVersion}) is available.\n\nCurrent version: {args.InstalledVersion}\n\nDo you want to update now?",
                            PrimaryButtonText = "Update",
                            CloseButtonText = "Later",
                            XamlRoot = this.Content.XamlRoot
                        };

                        var result = await dialog.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            try
                            {
                                AutoUpdater.DownloadUpdate(args);
                            }
                            catch
                            {
                                // Handle or ignore download errors
                            }
                        }
                    }
                };

                AutoUpdater.Start(tempFilePath);
            });
        }
        catch
        {
            // Ignore update check failures silently
        }
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
