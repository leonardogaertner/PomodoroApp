namespace PomodoroApp
{
    public partial class Knowledge : ContentPage
    {
        private readonly HttpClient _httpClient;

        public Knowledge()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PomodoroApp/1.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        private async void AcquireKnowledge_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Show loading state
                AdviceLabel.Text = "Fetching advice...";
                AdviceFrame.IsVisible = true;
                AcquireKnowledgeButton.IsEnabled = false;
                System.Diagnostics.Debug.WriteLine("Fetching advice from API...");

                // Fetch advice from API
                string json = await _httpClient.GetStringAsync("https://api.adviceslip.com/advice");
                System.Diagnostics.Debug.WriteLine($"API response: {json}");

                // Deserialize JSON with case-insensitive property names
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var adviceResponse = System.Text.Json.JsonSerializer.Deserialize<AdviceSlipResponse>(json, options);

                // Update UI with advice
                if (adviceResponse?.Slip?.Advice != null)
                {
                    AdviceLabel.Text = adviceResponse.Slip.Advice;
                    SemanticProperties.SetDescription(AdviceLabel, $"Productivity advice: {adviceResponse.Slip.Advice}");
                    AdviceFrame.IsVisible = true;
                    System.Diagnostics.Debug.WriteLine($"Advice displayed: {adviceResponse.Slip.Advice}");
                }
                else
                {
                    AdviceLabel.Text = "No advice received. Try again.";
                    await DisplayAlert("Error", "Failed to retrieve advice. Try again later.", "OK");
                    System.Diagnostics.Debug.WriteLine("API returned null or invalid advice.");
                }
            }
            catch (HttpRequestException ex)
            {
                AdviceLabel.Text = "Network error. Try again.";
                await DisplayAlert("Error", $"Network error: {ex.Message}. Check your internet connection.", "OK");
                System.Diagnostics.Debug.WriteLine($"Network error: {ex}");
            }
            catch (System.Text.Json.JsonException ex)
            {
                AdviceLabel.Text = "Error parsing advice. Try again.";
                await DisplayAlert("Error", "Failed to parse advice from the server. Try again later.", "OK");
                System.Diagnostics.Debug.WriteLine($"JSON parsing error: {ex}");
            }
            catch (Exception ex)
            {
                AdviceLabel.Text = "Unexpected error. Try again.";
                await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex}");
            }
            finally
            {
                AcquireKnowledgeButton.IsEnabled = true;
            }
        }

        // Model for JSON deserialization
        private class AdviceSlipResponse
        {
            public AdviceSlip Slip { get; set; }
        }

        private class AdviceSlip
        {
            public int Id { get; set; }
            public string Advice { get; set; }
        }
    }
}