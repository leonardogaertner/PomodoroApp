namespace PomodoroApp
{
    public partial class MainPage : ContentPage
    {
        private const int PomodoroDurationSeconds = 25 * 60; // 25 minutes
        private int totalSeconds = PomodoroDurationSeconds;
        private bool isRunning = false;
        private IDispatcherTimer timer;

        public MainPage()
        {
            InitializeComponent();

            // Initialize the timer
            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            // Set initial timer display
            UpdateTimerDisplay();
            UpdateButtonStates();
        }

        // Updates the timer display
        private void UpdateTimerDisplay()
        {
            TimerLabel.Text = TimeSpan.FromSeconds(totalSeconds).ToString(@"mm\:ss");
            SemanticProperties.SetDescription(TimerLabel, $"Time remaining: {TimeSpan.FromSeconds(totalSeconds).ToString(@"mm\:ss")}");
        }

        // Updates button enabled states based on timer state
        private void UpdateButtonStates()
        {
            StartButton.IsEnabled = !isRunning;
            PauseButton.IsEnabled = isRunning;
            ResetButton.IsEnabled = totalSeconds < PomodoroDurationSeconds || isRunning;
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
                // Timer finished
                isRunning = false;
                timer.Stop();
                UpdateButtonStates();
                DisplayAlert("Pomodoro Complete", "Great job! Take a break.", "OK");
            }
        }

        // Start button click handler
        private void StartTimer_Clicked(object sender, EventArgs e)
        {
            if (isRunning) return; // Prevent restarting an already running timer

            isRunning = true;
            timer.Start();
            UpdateButtonStates();
        }

        // Pause button click handler
        private void PauseTimer_Clicked(object sender, EventArgs e)
        {
            if (!isRunning) return; // Prevent pausing a stopped timer

            isRunning = false;
            timer.Stop();
            UpdateButtonStates();
        }

        // Reset button click handler
        private void ResetTimer_Clicked(object sender, EventArgs e)
        {
            isRunning = false;
            timer.Stop();
            totalSeconds = PomodoroDurationSeconds;
            UpdateTimerDisplay();
            UpdateButtonStates();
        }
    }
}