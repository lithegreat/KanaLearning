using Microsoft.UI.Xaml;
using KanaLearning.Services;
using KanaLearning.ViewModels;

namespace KanaLearning;

public partial class App : Application
{
    private Window? _window;

    public static App CurrentApp => (App)Current;

    public LocalizationService LocalizationService { get; }

    public QuestionImportService QuestionImportService { get; }

    public QuizEvaluationService QuizEvaluationService { get; }

    public QuizViewModel QuizViewModel { get; }

    public SettingsViewModel SettingsViewModel { get; }

    public Window MainWindow { get; private set; } = null!;

    public App()
    {
        InitializeComponent();

        LocalizationService = new LocalizationService();
        QuestionImportService = new QuestionImportService();
        QuizEvaluationService = new QuizEvaluationService();

        QuizViewModel = new QuizViewModel(LocalizationService, QuestionImportService, QuizEvaluationService);
        SettingsViewModel = new SettingsViewModel(LocalizationService);
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow(QuizViewModel, SettingsViewModel, LocalizationService);
        MainWindow = _window;
        _window.Activate();
    }
}
