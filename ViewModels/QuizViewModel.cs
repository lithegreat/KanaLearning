using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KanaLearning.Core.Services;
using KanaLearning.Models;
using KanaLearning.Services;

namespace KanaLearning.ViewModels;

public partial class QuizViewModel : ObservableObject
{
    private readonly ILocalizationService _localizationService;
    private readonly IQuestionImportService _questionImportService;
    private readonly IQuizEvaluationService _quizEvaluationService;
    private readonly Random _random = new();
    private readonly List<KanaQuestion> _allQuestions = new();

    public QuizViewModel(
        ILocalizationService localizationService,
        IQuestionImportService questionImportService,
        IQuizEvaluationService quizEvaluationService)
    {
        _localizationService = localizationService;
        _questionImportService = questionImportService;
        _quizEvaluationService = quizEvaluationService;

        _localizationService.LanguageChanged += OnLanguageChanged;

        CategoryOptions = new ObservableCollection<CategoryOption>();
        RebuildCategoryOptions();

        IncludeHiragana = true;
        IncludeKatakana = true;

        LoadDefaultQuestions();
        SelectNextQuestion();
    }

    public ObservableCollection<CategoryOption> CategoryOptions { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentKanaText))]
    [NotifyPropertyChangedFor(nameof(HasCurrentQuestion))]
    public partial KanaQuestion? CurrentQuestion { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasFeedback))]
    public partial string FeedbackMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string BankFeedbackMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string UserAnswer { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    [NotifyPropertyChangedFor(nameof(AccuracyText))]
    public partial int AttemptedCount { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AccuracyText))]
    public partial int CorrectCount { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredCountText))]
    public partial bool IncludeHiragana { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredCountText))]
    public partial bool IncludeKatakana { get; set; }

    [ObservableProperty]
    public partial string ManualKana { get; set; } = string.Empty;

    [ObservableProperty]
    public partial CategoryOption? SelectedManualCategory { get; set; }

    public string PageTitleText => _localizationService.GetString("Quiz.Title");

    public string CurrentKanaLabelText => _localizationService.GetString("Quiz.CurrentKana");

    public string CurrentKanaText => CurrentQuestion?.Kana ?? _localizationService.GetString("Quiz.NoQuestion");

    public bool HasCurrentQuestion => CurrentQuestion is not null;

    public string AnswerLabelText => _localizationService.GetString("Quiz.Answer");

    public string AnswerPlaceholderText => _localizationService.GetString("Quiz.Answer.Placeholder");

    public string SubmitButtonText => _localizationService.GetString("Quiz.Submit");

    public string NextButtonText => _localizationService.GetString("Quiz.Next");

    public string ImportButtonText => _localizationService.GetString("Quiz.Import");

    public string AddButtonText => _localizationService.GetString("Quiz.AddQuestion");

    public string ResetButtonText => _localizationService.GetString("Quiz.Reset");

    public string ImportHintText => _localizationService.GetString("Quiz.ImportHint");

    public string ManualInputTitleText => _localizationService.GetString("Quiz.ManualInput");

    public string ManualKanaLabelText => _localizationService.GetString("Quiz.ManualKana");

    public string ManualRomajiLabelText => _localizationService.GetString("Quiz.ManualRomaji");

    public string ManualCategoryLabelText => _localizationService.GetString("Quiz.ManualCategory");

    public string FiltersTitleText => _localizationService.GetString("Quiz.Filters");

    public string FilterHiraganaText => _localizationService.GetString("Quiz.Filter.Hiragana");

    public string FilterKatakanaText => _localizationService.GetString("Quiz.Filter.Katakana");

    public bool HasFeedback => !string.IsNullOrWhiteSpace(FeedbackMessage);

    public string ProgressText => string.Format(
        _localizationService.GetString("Quiz.Progress"),
        AttemptedCount,
        CorrectCount);

    public string AccuracyText
    {
        get
        {
            if (AttemptedCount == 0)
            {
                return string.Format(_localizationService.GetString("Quiz.Accuracy"), "0.00");
            }

            double accuracy = (double)CorrectCount / AttemptedCount * 100;
            return string.Format(_localizationService.GetString("Quiz.Accuracy"), accuracy.ToString("F2"));
        }
    }

    public string FilteredCountText
    {
        get
        {
            int count = GetFilteredQuestions().Count;
            return string.Format(_localizationService.GetString("Quiz.Available"), count);
        }
    }

    [RelayCommand]
    private void SubmitAnswer()
    {
        if (CurrentQuestion is null)
        {
            FeedbackMessage = _localizationService.GetString("Quiz.NoQuestion");
            return;
        }

        AttemptedCount++;
        bool isCorrect = _quizEvaluationService.IsCorrect(CurrentQuestion, UserAnswer);
        if (isCorrect)
        {
            CorrectCount++;
            FeedbackMessage = string.Format(
                _localizationService.GetString("Quiz.Feedback.Correct"),
                CurrentQuestion.Romaji);
        }
        else
        {
            FeedbackMessage = string.Format(
                _localizationService.GetString("Quiz.Feedback.Incorrect"),
                CurrentQuestion.Romaji);
        }

        UserAnswer = string.Empty;
    }

    [RelayCommand]
    private void NextQuestion()
    {
        SelectNextQuestion();
    }

    [RelayCommand]
    private void AddQuestion()
    {
        if (string.IsNullOrWhiteSpace(ManualKana))
        {
            BankFeedbackMessage = _localizationService.GetString("Quiz.Feedback.InvalidManualInput");
            return;
        }
        
        string kana = ManualKana.Trim();
        string? romaji = KanaMapper.GetRomaji(kana);
        
        if (string.IsNullOrEmpty(romaji))
        {
            BankFeedbackMessage = _localizationService.GetString("Quiz.Feedback.InvalidManualInput");
            return;
        }

        KanaCategory category = SelectedManualCategory?.Category ?? KanaCategory.Hiragana;
        KanaQuestion newQuestion = new()
        {
            Kana = kana,
            Romaji = romaji,
            Category = category,
        };

        if (_allQuestions.Any(q =>
                string.Equals(q.Kana, newQuestion.Kana, StringComparison.Ordinal) &&
                string.Equals(q.Romaji, newQuestion.Romaji, StringComparison.OrdinalIgnoreCase)))
        {
            BankFeedbackMessage = _localizationService.GetString("Quiz.Feedback.Duplicate");
            return;
        }

        _allQuestions.Add(newQuestion);
        ManualKana = string.Empty;
        BankFeedbackMessage = _localizationService.GetString("Quiz.Feedback.ManualAdded");
        SelectNextQuestion();
        OnPropertyChanged(nameof(FilteredCountText));
    }

    [RelayCommand]
    private void ResetSession()
    {
        AttemptedCount = 0;
        CorrectCount = 0;
        FeedbackMessage = string.Empty;
        SelectNextQuestion();
    }

    public async Task ImportFromPathAsync(string path)
    {
        var result = await _questionImportService.ImportFromPathAsync(path).ConfigureAwait(false);

        if (result.Questions.Count == 0)
        {
            BankFeedbackMessage = result.Errors.Count > 0
                ? result.Errors[0]
                : _localizationService.GetString("Quiz.Feedback.ImportEmpty");
            return;
        }

        int added = 0;
        foreach (KanaQuestion question in result.Questions)
        {
            if (_allQuestions.Any(q =>
                    string.Equals(q.Kana, question.Kana, StringComparison.Ordinal) &&
                    string.Equals(q.Romaji, question.Romaji, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            _allQuestions.Add(question);
            added++;
        }

        if (added == 0)
        {
            BankFeedbackMessage = _localizationService.GetString("Quiz.Feedback.DuplicateImport");
            return;
        }

        BankFeedbackMessage = string.Format(_localizationService.GetString("Quiz.Feedback.ImportSuccess"), added);
        SelectNextQuestion();
        OnPropertyChanged(nameof(FilteredCountText));
    }

    partial void OnIncludeHiraganaChanged(bool value)
    {
        OnFilterChanged();
    }

    partial void OnIncludeKatakanaChanged(bool value)
    {
        OnFilterChanged();
    }

    private void OnFilterChanged()
    {
        OnPropertyChanged(nameof(FilteredCountText));
        SelectNextQuestion();
    }

    private void SelectNextQuestion()
    {
        List<KanaQuestion> filtered = GetFilteredQuestions();
        if (filtered.Count == 0)
        {
            CurrentQuestion = null;
            return;
        }

        int index = _random.Next(filtered.Count);
        CurrentQuestion = filtered[index];
        UserAnswer = string.Empty;
    }

    private List<KanaQuestion> GetFilteredQuestions()
    {
        return _allQuestions
            .Where(question =>
            {
                if (string.IsNullOrEmpty(question.Kana)) return false;
                char firstChar = question.Kana[0];
                bool isHiragana = firstChar >= '\u3040' && firstChar <= '\u309F';
                bool isKatakana = firstChar >= '\u30A0' && firstChar <= '\u30FF';

                return (IncludeHiragana && isHiragana) || (IncludeKatakana && isKatakana);
            })
            .ToList();
    }

    private void LoadDefaultQuestions()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Kana", "default-kana.json");
        if (!File.Exists(filePath))
        {
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            KanaQuestion[]? questions = JsonSerializer.Deserialize<KanaQuestion[]>(json);
            if (questions is null)
            {
                return;
            }

            foreach (KanaQuestion question in questions)
            {
                if (string.IsNullOrWhiteSpace(question.Kana) || string.IsNullOrWhiteSpace(question.Romaji))
                {
                    continue;
                }

                _allQuestions.Add(question);
            }
        }
        catch (JsonException)
        {
            // Keep app usable even if sample data is malformed.
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        RebuildCategoryOptions();
        OnPropertyChanged(string.Empty);
    }

    private void RebuildCategoryOptions()
    {
        KanaCategory? selectedCategory = SelectedManualCategory?.Category;

        CategoryOptions.Clear();
        CategoryOptions.Add(new CategoryOption
        {
            Category = KanaCategory.Hiragana,
            DisplayName = _localizationService.GetString("Category.Hiragana"),
        });
        CategoryOptions.Add(new CategoryOption
        {
            Category = KanaCategory.Katakana,
            DisplayName = _localizationService.GetString("Category.Katakana"),
        });


        CategoryOption? match = CategoryOptions.FirstOrDefault(c => c.Category == selectedCategory);
        SelectedManualCategory = match ?? CategoryOptions.FirstOrDefault();
    }
}
