using Microsoft.UI.Xaml.Controls;
using KanaLearning.ViewModels;

namespace KanaLearning.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        ViewModel = App.CurrentApp.SettingsViewModel;
        InitializeComponent();
        DataContext = ViewModel;
    }

    public SettingsViewModel ViewModel { get; private set; }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        if (e.Parameter is SettingsViewModel viewModel)
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
}
