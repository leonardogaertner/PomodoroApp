// PomodoroApp/ViewModels/SetupViewModel.cs

using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace PomodoroApp.ViewModels
{
    public partial class SetupViewModel : INotifyPropertyChanged
    {
        private string _focusTime;
        private string _shortBreak;
        private string _longBreak;
        private string _repetitions;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SaveSettingsCommand { get; }

        public SetupViewModel()
        {
            LoadSettings();
            SaveSettingsCommand = new Command(async () => await SaveSettings());
        }

        private void LoadSettings()
        {
            FocusTime = Preferences.Get("FocusTime", 25).ToString();
            ShortBreak = Preferences.Get("ShortBreak", 5).ToString();
            LongBreak = Preferences.Get("LongBreak", 15).ToString();
            Repetitions = Preferences.Get("Repetitions", 4).ToString();
        }

        private async Task SaveSettings()
        {
            if (!int.TryParse(FocusTime, out int focusTime) || focusTime <= 0)
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, insira um tempo de foco válido (número positivo).", "OK");
                return;
            }
            if (!int.TryParse(ShortBreak, out int shortBreak) || shortBreak <= 0)
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, insira um tempo de pausa curta válido (número positivo).", "OK");
                return;
            }
            if (!int.TryParse(LongBreak, out int longBreak) || longBreak <= 0)
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, insira um tempo de pausa longa válido (número positivo).", "OK");
                return;
            }
            if (!int.TryParse(Repetitions, out int repetitions) || repetitions <= 0)
            {
                await Shell.Current.DisplayAlert("Erro", "Por favor, insira um número de repetições válido (número positivo).", "OK");
                return;
            }

            Preferences.Set("FocusTime", focusTime);
            Preferences.Set("ShortBreak", shortBreak);
            Preferences.Set("LongBreak", longBreak);
            Preferences.Set("Repetitions", repetitions);

            await Shell.Current.DisplayAlert("Sucesso", "Configurações salvas com sucesso!", "OK");
            await Shell.Current.GoToAsync("..");
        }

        // Propriedades vinculáveis
        public string FocusTime
        {
            get => _focusTime;
            set
            {
                _focusTime = value;
                OnPropertyChanged(nameof(FocusTime));
            }
        }

        public string ShortBreak
        {
            get => _shortBreak;
            set
            {
                _shortBreak = value;
                OnPropertyChanged(nameof(ShortBreak));
            }
        }

        public string LongBreak
        {
            get => _longBreak;
            set
            {
                _longBreak = value;
                OnPropertyChanged(nameof(LongBreak));
            }
        }

        public string Repetitions
        {
            get => _repetitions;
            set
            {
                _repetitions = value;
                OnPropertyChanged(nameof(Repetitions));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}