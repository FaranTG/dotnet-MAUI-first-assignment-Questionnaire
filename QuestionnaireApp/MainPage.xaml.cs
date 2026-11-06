namespace QuestionnaireApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        ThemeSwitch.IsToggled = Application.Current?.RequestedTheme == AppTheme.Dark;

        TeaTypePicker.SelectedIndex = 0;
        BrewMethodPicker.SelectedIndex = 0;

        ValidateForm();
    }

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        Application.Current?.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
    }

    private void OnCalendarButtonClicked(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            HiddenDatePicker.Unfocus(); 
            HiddenDatePicker.Focus();
        });
    }

    private void OnDateSelectedFromCalendar(object sender, DateChangedEventArgs e)
    {
        DateEntry.Text = (e.NewDate ?? DateTime.Today).ToString("dd.MM.yyyy");
    }

    private void OnRequiredFieldsChanged(object sender, TextChangedEventArgs e)
    {
        ValidateForm();
    }

    private void OnLabelTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is CheckBox checkBox)
        {
            checkBox.IsChecked = !checkBox.IsChecked;
        }
    }

    private void ValidateForm()
    {
        bool isTesterFilled = !string.IsNullOrWhiteSpace(TesterNameEntry?.Text);
        bool isEmailFilled = !string.IsNullOrWhiteSpace(TesterEmailEntry?.Text);
        bool isTeaFilled = !string.IsNullOrWhiteSpace(TeaNameEntry?.Text);

        TesterNameEntry?.PlaceholderColor = isTesterFilled ? Colors.Gray :  Colors.Red;
        TesterEmailEntry?.PlaceholderColor = isEmailFilled ? Colors.Gray :  Colors.Red;
        TeaNameEntry?.PlaceholderColor = isTeaFilled ? Colors.Gray :  Colors.Red;
        
        bool isFormValid = isTesterFilled && isEmailFilled && isTeaFilled;

        if (SubmitButton != null)
        {
            SubmitButton.IsEnabled = isFormValid;
            SubmitButton.BackgroundColor = isFormValid 
                ? Colors.Orange 
                : Color.FromArgb("#AEAEB2");
        }

        ValidationWarningLabel?.IsVisible = !isFormValid;
    }

    private async void OnSubmitFormClicked(object sender, EventArgs e)
    {
        string testerName = TesterNameEntry.Text.Trim();
        string testerEmail = TesterEmailEntry.Text.Trim();
        string teaName = TeaNameEntry.Text.Trim();
        
        string tastingDate = string.IsNullOrWhiteSpace(DateEntry.Text) 
            ? DateTime.Today.ToString("dd.MM.yyyy") 
            : DateEntry.Text.Trim();

        string teaType = TeaTypePicker.SelectedIndex > 0 
            ? TeaTypePicker.SelectedItem?.ToString() ?? "Не указан" 
            : "Не указан";

        string brewMethod = BrewMethodPicker.SelectedIndex > 0 
            ? BrewMethodPicker.SelectedItem?.ToString() ?? "Не указан" 
            : "Не указан";

        int finalScore = (int)Math.Round(ScoreSlider.Value);

        string teaEffect = BodrostRadio.IsChecked
            ? "Бодрость"
            : FocusRadio.IsChecked
                ? "Концентрация"
                : "Расслабление";

        List<string> supplements = [];
        if (SugarCheckBox.IsChecked) supplements.Add("Подсластитель");
        if (MilkCheckBox.IsChecked) supplements.Add("Молоко");
        if (LemonCheckBox.IsChecked) supplements.Add("Лимон");

        string supplementsResult = supplements.Count > 0 
            ? string.Join(", ", supplements) 
            : "-";

        string reviewNotes = string.IsNullOrWhiteSpace(ReviewEditor.Text) 
            ? "-" 
            : ReviewEditor.Text.Trim();

        ResultSummaryLabel.Text = $"""
            Дегустатор: {testerName} ({testerEmail})
            Дата сессии: {tastingDate}
            Оцениваемый сорт: {teaName}
            Категория: {teaType}
            Способ заварки: {brewMethod}
            Оценка вкуса: {finalScore} из 10
            Эффект: {teaEffect}
            Добавки: {supplementsResult}
            Заметки: {reviewNotes}
            """;

        ResultCard.IsVisible = true;
        await DisplayAlertAsync("Успех", "Форма успешно отправлена! Итоги дегустации можно увидеть ниже.", "Отлично");
    }

    private void OnClearFormClicked(object sender, EventArgs e)
    {
        TesterNameEntry.Text = string.Empty;
        TesterEmailEntry.Text = string.Empty;
        TeaNameEntry.Text = string.Empty;
        ReviewEditor.Text = string.Empty;
        DateEntry.Text = string.Empty;

        TeaTypePicker.SelectedIndex = 0;
        BrewMethodPicker.SelectedIndex = 0;

        HiddenDatePicker.Date = DateTime.Today;
        ScoreSlider.Value = 5;

        BodrostRadio.IsChecked = true;
        SugarCheckBox.IsChecked = false;
        MilkCheckBox.IsChecked = false;
        LemonCheckBox.IsChecked = false;

        ResultCard.IsVisible = false;
        ResultSummaryLabel.Text = string.Empty;

        ValidateForm();
    }
}
