namespace PomodoroApp;

public partial class Setup : ContentPage
{
    public Setup()
    {
        InitializeComponent();
        LoadSettings();
    }

    // Load saved settings into the entry fields
    private void LoadSettings()
    {
        FocusTimeEntry.Text = Preferences.Get("FocusTime", 25).ToString();
        ShortBreakEntry.Text = Preferences.Get("ShortBreak", 5).ToString();
        LongBreakEntry.Text = Preferences.Get("LongBreak", 15).ToString();
        RepetitionsEntry.Text = Preferences.Get("Repetitions", 4).ToString();
    }

    // Save button click handler
    private async void SaveSettings_Clicked(object sender, EventArgs e)
    {
        // Validate inputs
        if (!int.TryParse(FocusTimeEntry.Text, out int focusTime) || focusTime <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid focus time (positive number).", "OK");
            return;
        }
        if (!int.TryParse(ShortBreakEntry.Text, out int shortBreak) || shortBreak <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid short break time (positive number).", "OK");
            return;
        }
        if (!int.TryParse(LongBreakEntry.Text, out int longBreak) || longBreak <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid long break time (positive number).", "OK");
            return;
        }
        if (!int.TryParse(RepetitionsEntry.Text, out int repetitions) || repetitions <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid number of repetitions (positive number).", "OK");
            return;
        }

        // Save settings to Preferences
        Preferences.Set("FocusTime", focusTime);
        Preferences.Set("ShortBreak", shortBreak);
        Preferences.Set("LongBreak", longBreak);
        Preferences.Set("Repetitions", repetitions);

        await DisplayAlert("Success", "Settings saved successfully!", "OK");
        // Optionally navigate back to MainPage
        await Navigation.PopAsync();
    }
}