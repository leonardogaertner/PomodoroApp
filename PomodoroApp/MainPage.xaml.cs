namespace PomodoroApp
{
    public partial class MainPage : ContentPage
    {
        private int focusTimeSeconds; // Loaded from Preferences
        private int shortBreakSeconds;
        private int longBreakSeconds;
        private int repetitions;
        private int currentCycle = 0; // Tracks current Pomodoro cycle
        private int totalSeconds; // Current timer duration
        private bool isRunning = false;
        private bool isFocusSession = true; // True for focus, false for break
        private IDispatcherTimer timer;

        public MainPage()
        {
            InitializeComponent();
            InitializeTimer();
            LoadSettings();
            totalSeconds = focusTimeSeconds; // Start with focus time
            UpdateTimerDisplay();
            UpdateButtonStates();
        }

        // Called when the page appears (e.g., after returning from Setup)
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSettings();
            ResetTimerToFocus(); // Reset to focus session with new settings
            System.Diagnostics.Debug.WriteLine("MainPage appeared, settings reloaded.");
        }

        // Initialize the timer
        private void InitializeTimer()
        {
            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        // Load settings from Preferences with defaults
        private void LoadSettings()
        {
            focusTimeSeconds = Preferences.Get("FocusTime", 25) * 60;
            shortBreakSeconds = Preferences.Get("ShortBreak", 5) * 60;
            longBreakSeconds = Preferences.Get("LongBreak", 15) * 60;
            repetitions = Preferences.Get("Repetitions", 4);

            // Ensure valid settings
            if (focusTimeSeconds <= 0) focusTimeSeconds = 25 * 60;
            if (shortBreakSeconds <= 0) shortBreakSeconds = 5 * 60;
            if (longBreakSeconds <= 0) longBreakSeconds = 15 * 60;
            if (repetitions <= 0) repetitions = 4;

            System.Diagnostics.Debug.WriteLine($"Loaded settings: Focus={focusTimeSeconds / 60}m, ShortBreak={shortBreakSeconds / 60}m, LongBreak={longBreakSeconds / 60}m, Repetitions={repetitions}");
        }

        // Reset timer to focus session
        private void ResetTimerToFocus()
        {
            isRunning = false;
            timer?.Stop();
            isFocusSession = true;
            currentCycle = 0;
            totalSeconds = focusTimeSeconds;
            UpdateTimerDisplay();
            UpdateButtonStates();
        }

        // Updates the timer display
        private void UpdateTimerDisplay()
        {
            TimerLabel.Text = TimeSpan.FromSeconds(totalSeconds).ToString(@"mm\:ss");
            string sessionType = isFocusSession ? "Focus" : "Break";
            SemanticProperties.SetDescription(TimerLabel, $"{sessionType} time remaining: {TimeSpan.FromSeconds(totalSeconds).ToString(@"mm\:ss")}");
        }

        // Updates button enabled states based on timer state
        private void UpdateButtonStates()
        {
            StartButton.IsEnabled = !isRunning;
            PauseButton.IsEnabled = isRunning;
            ResetButton.IsEnabled = totalSeconds < (isFocusSession ? focusTimeSeconds : (currentCycle < repetitions ? shortBreakSeconds : longBreakSeconds)) || isRunning;
        }

        // Timer tick event handler
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isRunning) return;

            if (totalSeconds > 0)
            {
                totalSeconds--;
                UpdateTimerDisplay();
            }
            else
            {
                isRunning = false;
                timer.Stop();

                if (isFocusSession)
                {
                    currentCycle++;
                    if (currentCycle < repetitions)
                    {
                        // Start short break
                        isFocusSession = false;
                        totalSeconds = shortBreakSeconds;
                        DisplayAlert("Focus Complete", "Time for a short break!", "OK");
                    }
                    else
                    {
                        // Start long break and reset cycle
                        isFocusSession = false;
                        totalSeconds = longBreakSeconds;
                        currentCycle = 0;
                        DisplayAlert("Focus Complete", "Time for a long break!", "OK");
                    }
                }
                else
                {
                    // Break finished, start next focus session
                    isFocusSession = true;
                    totalSeconds = focusTimeSeconds;
                    DisplayAlert("Break Complete", "Time to focus again!", "OK");
                }

                UpdateTimerDisplay();
                UpdateButtonStates();
            }
        }

        // Start button click handler
        private void StartTimer_Clicked(object sender, EventArgs e)
        {
            if (isRunning)
            {
                System.Diagnostics.Debug.WriteLine("Start button clicked while timer is running.");
                return;
            }

            if (timer == null)
            {
                InitializeTimer();
                System.Diagnostics.Debug.WriteLine("Timer was null, reinitialized.");
            }

            isRunning = true;
            timer.Start();
            UpdateButtonStates();
            System.Diagnostics.Debug.WriteLine("Timer started.");
        }

        // Pause button click handler
        private void PauseTimer_Clicked(object sender, EventArgs e)
        {
            if (!isRunning) return;

            isRunning = false;
            timer.Stop();
            UpdateButtonStates();
            System.Diagnostics.Debug.WriteLine("Timer paused.");
        }

        // Reset button click handler
        private void ResetTimer_Clicked(object sender, EventArgs e)
        {
            isRunning = false;
            timer?.Stop();
            totalSeconds = isFocusSession ? focusTimeSeconds : (currentCycle < repetitions ? shortBreakSeconds : longBreakSeconds);
            UpdateTimerDisplay();
            UpdateButtonStates();
            System.Diagnostics.Debug.WriteLine("Timer reset.");
        }

        // Settings button click handler
        private async void GoToSettings_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Setup());
            System.Diagnostics.Debug.WriteLine("Navigated to Setup page.");
        }
    }
}