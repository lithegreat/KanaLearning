using Microsoft.UI.Xaml;
using KanaLearning.Services;
using KanaLearning.UIServices;
using KanaLearning.ViewModels;

namespace KanaLearning;

public partial class App : Application
{
    private Window? _window;

    public static App CurrentApp => (App)Current;

    public LocalizationService LocalizationService { get; }

    public QuestionImportService QuestionImportService { get; }
    public QuestionExportService QuestionExportService { get; }

    public QuizEvaluationService QuizEvaluationService { get; }

    public QuizViewModel QuizViewModel { get; }

    public SettingsViewModel SettingsViewModel { get; }

    public ThemeService ThemeService { get; }

    public Window MainWindow { get; private set; } = null!;

    public App()
    {
        InitializeComponent();

        LocalizationService = new LocalizationService();
        QuestionImportService = new QuestionImportService();
        QuestionExportService = new QuestionExportService();
        QuizEvaluationService = new QuizEvaluationService();
        ThemeService = new ThemeService();

        QuizViewModel = new QuizViewModel(LocalizationService, QuestionImportService, QuestionExportService, QuizEvaluationService);
        SettingsViewModel = new SettingsViewModel(LocalizationService, ThemeService);
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow(QuizViewModel, SettingsViewModel, LocalizationService);
        MainWindow = _window;
        
        ThemeService.Initialize();
        
        _window.Activate();
    }
}
