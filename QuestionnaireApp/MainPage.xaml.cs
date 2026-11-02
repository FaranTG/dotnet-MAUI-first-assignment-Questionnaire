using System.Text;

namespace QuestionnaireApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        ThemeSwitch.IsToggled = Application.Current?.RequestedTheme == AppTheme.Dark;
    }

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        Application.Current?.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
    }

    private async void OnSubmitFormClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TesterNameEntry.Text) || string.IsNullOrWhiteSpace(TeaNameEntry.Text))
        {
            await DisplayAlertAsync("Ошибка валидации", "Пожалуйста, заполните обязательные поля:\n• Дегустатор\n• Сорт чая", "ОК");
            return;
        }

        string testerName = TesterNameEntry.Text?.Trim() ?? string.Empty;
        string teaName = TeaNameEntry.Text?.Trim() ?? string.Empty;
        string tastingDate = TastingDatePicker.Date?.ToString("dd.MM.yyyy") ?? DateTime.Today.ToString("dd.MM.yyyy");

        string teaType = TeaTypePicker.SelectedItem?.ToString() ?? "Не указан";
        string brewMethod = BrewMethodPicker.SelectedItem?.ToString() ?? "Не указан";

        int finalScore = (int)Math.Round(ScoreSlider.Value);

        string teaEffect = "Не выбрано";
        if (BodrostRadio.IsChecked) teaEffect = "Бодрость / Физический тонус";
        else if (FocusRadio.IsChecked) teaEffect = "Фокус / Концентрация";
        else if (RelaxRadio.IsChecked) teaEffect = "Расслабление / Медитация";

        var supplements = new List<string>();
        if (SugarCheckBox.IsChecked) supplements.Add("Подсластитель (Сахар/Мед)");
        if (MilkCheckBox.IsChecked) supplements.Add("Молоко/Сливки");
        if (LemonCheckBox.IsChecked) supplements.Add("Лимон/Цитрус");

        string supplementsResult = supplements.Count > 0 
            ? string.Join(", ", supplements) 
            : "Чистый вкус (без добавок)";

        string reviewNotes = string.IsNullOrWhiteSpace(ReviewEditor.Text) 
            ? "Заметки отсутствуют." 
            : ReviewEditor.Text.Trim();

        var summary = new StringBuilder();
        summary.AppendLine($"Сомелье: {testerName}");
        summary.AppendLine($"Дата сессии: {tastingDate}");
        summary.AppendLine($"Оцениваемый сорт: {teaName}");
        summary.AppendLine($"Категория: {teaType}");
        summary.AppendLine($"Способ заварки: {brewMethod}");
        summary.AppendLine($"Оценка вкуса: {finalScore} из 10");
        summary.AppendLine($"Эффект: {teaEffect}");
        summary.AppendLine($"Добавки: {supplementsResult}");
        summary.AppendLine($"Заметки: {reviewNotes}");

        ResultSummaryLabel.Text = summary.ToString();
        ResultCard.IsVisible = true;

        await DisplayAlertAsync("Успех", "Лист дегустации успешно сформирован!", "Отлично");
    }

    private void OnClearFormClicked(object sender, EventArgs e)
    {
        TesterNameEntry.Text = string.Empty;
        TeaNameEntry.Text = string.Empty;
        ReviewEditor.Text = string.Empty;

        TeaTypePicker.SelectedIndex = -1;
        BrewMethodPicker.SelectedIndex = -1;

        TastingDatePicker.Date = DateTime.Today;
        ScoreSlider.Value = 5;

        BodrostRadio.IsChecked = true;

        SugarCheckBox.IsChecked = false;
        MilkCheckBox.IsChecked = false;
        LemonCheckBox.IsChecked = false;

        ResultCard.IsVisible = false;
        ResultSummaryLabel.Text = string.Empty;
    }
}
