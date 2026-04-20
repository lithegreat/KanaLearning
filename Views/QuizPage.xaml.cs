using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using WinRT.Interop;
using KanaLearning.ViewModels;

namespace KanaLearning.Views;

public sealed partial class QuizPage : Page
{
    public QuizPage()
    {
        ViewModel = App.CurrentApp.QuizViewModel;
        InitializeComponent();
        DataContext = ViewModel;
    }

    public QuizViewModel ViewModel { get; private set; }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        if (e.Parameter is QuizViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }
        else
        {
            DataContext = ViewModel;
        }

        base.OnNavigatedTo(e);
    }

    private async void OnImportButtonClick(object sender, RoutedEventArgs e)
    {
        FileOpenPicker picker = new();
        picker.FileTypeFilter.Add(".json");
        picker.FileTypeFilter.Add(".txt");

        IntPtr hwnd = WindowNative.GetWindowHandle(App.CurrentApp.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file is null)
        {
            return;
        }

        await ViewModel.ImportFromPathAsync(file.Path);
    }

    private async void OnExportButtonClick(object sender, RoutedEventArgs e)
    {
        FileSavePicker picker = new();
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeChoices.Add("JSON File", new List<string>() { ".json" });
        picker.SuggestedFileName = "kana-questions";

        IntPtr hwnd = WindowNative.GetWindowHandle(App.CurrentApp.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSaveFileAsync();
        if (file is null)
        {
            return;
        }

        await ViewModel.ExportToPathAsync(file.Path);
    }

    private void OnAnswerTextBoxKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel.HasFeedback)
            {
                if (ViewModel.NextQuestionCommand.CanExecute(null))
                {
                    ViewModel.NextQuestionCommand.Execute(null);
                    e.Handled = true;
                }
            }
            else if (ViewModel.SubmitAnswerCommand.CanExecute(null))
            {
                ViewModel.SubmitAnswerCommand.Execute(null);
                e.Handled = true;
            }
        }
    }

    private void OnManualKanaTextBoxKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel.AddQuestionCommand.CanExecute(null))
            {
                ViewModel.AddQuestionCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}
