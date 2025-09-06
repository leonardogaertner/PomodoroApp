// PomodoroApp/ViewModels/MainViewModel.cs

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace PomodoroApp.ViewModels
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        private int _focusTimeSeconds;
        private int _shortBreakSeconds;
        private int _longBreakSeconds;
        private int _repetitions;
        private int _currentCycle = 0;
        private int _totalSeconds;
        private bool _isRunning = false;
        private bool _isFocusSession = true;
        private IDispatcherTimer _timer;

        // Variável de apoio para a propriedade TimerText
        private string _timerText;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand StartCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand GoToSettingsCommand { get; private set; }

        public MainViewModel()
        {
            InitializeCommands();
            InitializeTimer();
            LoadSettings();
            _totalSeconds = _focusTimeSeconds;
            UpdateTimerDisplay();
        }

        private void InitializeCommands()
        {
            StartCommand = new Command(StartTimer);
            PauseCommand = new Command(PauseTimer);
            ResetCommand = new Command(ResetTimer);
            GoToSettingsCommand = new Command(async () => await GoToSettings());
        }

        private void InitializeTimer()
        {
            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        private void LoadSettings()
        {
            _focusTimeSeconds = Preferences.Get("FocusTime", 25) * 60;
            _shortBreakSeconds = Preferences.Get("ShortBreak", 5) * 60;
            _longBreakSeconds = Preferences.Get("LongBreak", 15) * 60;
            _repetitions = Preferences.Get("Repetitions", 4);
        }

        public void ReloadSettings()
        {
            LoadSettings();
            ResetTimerToFocus();
        }

        private void StartTimer()
        {
            if (_isRunning) return;
            _isRunning = true;
            _timer.Start();
            OnPropertyChanged(nameof(IsRunning));
        }

        private void PauseTimer()
        {
            if (!_isRunning) return;
            _isRunning = false;
            _timer.Stop();
            OnPropertyChanged(nameof(IsRunning));
        }

        private void ResetTimer()
        {
            _isRunning = false;
            _timer?.Stop();
            _isFocusSession = true;
            _currentCycle = 0;
            _totalSeconds = _focusTimeSeconds;
            UpdateTimerDisplay();
            OnPropertyChanged(nameof(IsRunning));
        }

        private void ResetTimerToFocus()
        {
            _isRunning = false;
            _timer?.Stop();
            _isFocusSession = true;
            _currentCycle = 0;
            _totalSeconds = _focusTimeSeconds;
            UpdateTimerDisplay();
            OnPropertyChanged(nameof(IsRunning));
        }

        private async Task GoToSettings()
        {
            await Shell.Current.GoToAsync("Setup");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_totalSeconds > 0)
            {
                _totalSeconds--;
                UpdateTimerDisplay();
            }
            else
            {
                _isRunning = false;
                _timer.Stop();

                if (_isFocusSession)
                {
                    _currentCycle++;
                    if (_currentCycle < _repetitions)
                    {
                        _isFocusSession = false;
                        _totalSeconds = _shortBreakSeconds;
                        Shell.Current.DisplayAlert("Foco Completo", "É hora de uma pausa curta!", "OK");
                    }
                    else
                    {
                        _isFocusSession = false;
                        _totalSeconds = _longBreakSeconds;
                        _currentCycle = 0;
                        Shell.Current.DisplayAlert("Foco Completo", "É hora de uma pausa longa!", "OK");
                    }
                }
                else
                {
                    _isFocusSession = true;
                    _totalSeconds = _focusTimeSeconds;
                    Shell.Current.DisplayAlert("Pausa Completa", "É hora de focar novamente!", "OK");
                }

                UpdateTimerDisplay();
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        private void UpdateTimerDisplay()
        {
            TimerText = TimeSpan.FromSeconds(_totalSeconds).ToString(@"mm\:ss");
        }

        // Propriedades vinculáveis
        public string TimerText
        {
            get => _timerText;
            set
            {
                _timerText = value;
                OnPropertyChanged(nameof(TimerText));
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}