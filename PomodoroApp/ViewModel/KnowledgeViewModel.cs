// PomodoroApp/ViewModels/KnowledgeViewModel.cs
// No topo do arquivo PomodoroApp/ViewModels/KnowledgeViewModel.cs
using PomodoroApp.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Input;

namespace PomodoroApp.ViewModels
{
    public partial class KnowledgeViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private string _adviceText;
        private bool _isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AcquireKnowledgeCommand { get; }

        public KnowledgeViewModel()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PomodoroApp/1.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);

            AcquireKnowledgeCommand = new Command(async () => await AcquireKnowledge());

            // Define o texto inicial
            AdviceText = "Clique no botão para obter um conselho!";
        }

        private async Task AcquireKnowledge()
        {
            try
            {
                IsBusy = true;
                AdviceText = "A buscar conselho...";

                string json = await _httpClient.GetStringAsync("https://api.adviceslip.com/advice");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var adviceResponse = JsonSerializer.Deserialize<AdviceSlipResponse>(json, options);

                if (adviceResponse?.Slip?.Advice != null)
                {
                    AdviceText = adviceResponse.Slip.Advice;
                }
                else
                {
                    AdviceText = "Nenhum conselho recebido. Tente novamente.";
                    await Shell.Current.DisplayAlert("Erro", "Falha ao recuperar o conselho. Tente novamente mais tarde.", "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                AdviceText = "Erro de rede. Tente novamente.";
                await Shell.Current.DisplayAlert("Erro", $"Erro de rede: {ex.Message}. Verifique sua conexão com a internet.", "OK");
            }
            catch (JsonException ex)
            {
                AdviceText = "Erro ao analisar o conselho. Tente novamente.";
                await Shell.Current.DisplayAlert("Erro", "Falha ao analisar o conselho do servidor. Tente novamente mais tarde.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Propriedades vinculáveis
        public string AdviceText
        {
            get => _adviceText;
            set
            {
                _adviceText = value;
                OnPropertyChanged(nameof(AdviceText));
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private class AdviceSlipResponse
        {
            public AdviceSlip Slip { get; set; }
        }

        private class AdviceSlip
        {
            public int Id { get; set; }
            public string Advice { get; set; }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}