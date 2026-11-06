using System.Text.RegularExpressions;

namespace QuestionnaireApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        ThemeSwitch.IsToggled = Application.Current?.RequestedTheme == AppTheme.Dark;

        ResetPickers();
        ValidateForm();
    }

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        Application.Current?.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
    }

    private void OnCalendarButtonClicked(object sender, EventArgs e)
    {
        HiddenDatePicker.Focus();
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
        string tastingDate = GetTastingDate();

        var (isValid, errorMessage) = ValidateInputData(testerEmail, tastingDate);

        if (!isValid)
        {
            await DisplayAlertAsync("Ошибка ввода", errorMessage, "ОК");
            return;
        }

        string teaType = GetSelectedPickerItem(TeaTypePicker);
        string brewMethod = GetSelectedPickerItem(BrewMethodPicker);

        int finalScore = (int)Math.Round(ScoreSlider.Value);
        string teaEffect = GetSelectedTeaEffect();
        string supplementsResult = GetSelectedSupplements();
        string reviewNotes = GetReviewNotes();

        ResultSummaryLabel.Text = BuildResultSummary(
            testerName, testerEmail, tastingDate, teaName, 
            teaType, brewMethod, finalScore, teaEffect, 
            supplementsResult, reviewNotes);

        ResultCard.IsVisible = true;
        await DisplayAlertAsync("Успех", "Форма успешно отправлена! Итоги дегустации можно увидеть ниже.", "Отлично");
    }

    private (bool IsValid, string ErrorMessage) ValidateInputData(string email, string date)
    {
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern))
        {
            return (false, "Указан некорректный формат почты! Пример: example@mail.com");
        }

        if (!Regex.IsMatch(date, @"^\d{2}\.\d{2}\.\d{4}$"))
        {
            return (false, "Неверный формат даты! Используйте формат ДД.ММ.ГГГГ");
        }

        if (!DateTime.TryParseExact(date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
        {
            return (false, "Введена несуществующая дата! Проверьте правильность дней и месяцев.");
        }

        DateTime minDate = new (2026, 10, 1);
        DateTime maxDate = new (2026, 12, 31);

        if (parsedDate < minDate || parsedDate > maxDate)
        {
            return (false, "Дата сессии должна быть в диапазоне от 01.10.2026 до 31.12.2026!");
        }

        return (true, string.Empty);
    }

    private string GetTastingDate()
    {
        return string.IsNullOrWhiteSpace(DateEntry.Text) 
            ? DateTime.Today.ToString("dd.MM.yyyy") 
            : DateEntry.Text.Trim();
    }

    private string GetSelectedPickerItem(Picker picker)
    {
        return picker.SelectedIndex > 0 
            ? picker.SelectedItem?.ToString() ?? "Не указан" 
            : "Не указан";
    }

    private string GetSelectedTeaEffect()
    {
        if (BodrostRadio.IsChecked) return "Бодрость";
        if (FocusRadio.IsChecked) return "Концентрация";
        return "Расслабление";
    }

    private string GetSelectedSupplements()
    {
        List<string> supplements = [];
        if (SugarCheckBox.IsChecked) supplements.Add("Подсластитель");
        if (MilkCheckBox.IsChecked) supplements.Add("Молоко");
        if (LemonCheckBox.IsChecked) supplements.Add("Лимон");

        return supplements.Count > 0 ? string.Join(", ", supplements) : "-";
    }

    private string GetReviewNotes()
    {
        return string.IsNullOrWhiteSpace(ReviewEditor.Text) ? "-" : ReviewEditor.Text.Trim();
    }

    private string BuildResultSummary(
        string name, string email, string date, string tea, 
        string type, string method, int score, string effect, 
        string supplements, string notes)
    {
        return $"""
            Дегустатор: {name} ({email})
            Дата сессии: {date}
            Оцениваемый сорт: {tea}
            Категория: {type}
            Способ заварки: {method}
            Оценка вкуса: {score} из 10
            Эффект: {effect}
            Добавки: {supplements}
            Заметки: {notes}
            """;
    }

    private void OnClearFormClicked(object sender, EventArgs e)
    {
        ResetEntries();

        HiddenDatePicker.Date = DateTime.Today;

        ScoreSlider.Value = 5;

        ResetPickers();
        ResetCheckBoxes();

        BodrostRadio.IsChecked = true;

        ReviewEditor.Text = string.Empty;

        ResultCard.IsVisible = false;
        ResultSummaryLabel.Text = string.Empty;

        ValidateForm();
    }

    private void ResetEntries()
    {
        TesterNameEntry.Text = string.Empty;
        TesterEmailEntry.Text = string.Empty;
        TeaNameEntry.Text = string.Empty;
        DateEntry.Text = string.Empty;
    }

    private void ResetPickers()
    {
        TeaTypePicker.SelectedIndex = 0;
        BrewMethodPicker.SelectedIndex = 0;
    }

    private void ResetCheckBoxes()
    {
        SugarCheckBox.IsChecked = false;
        MilkCheckBox.IsChecked = false;
        LemonCheckBox.IsChecked = false;
    }
}
