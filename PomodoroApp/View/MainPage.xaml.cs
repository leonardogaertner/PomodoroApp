// PomodoroApp/Views/MainPage.xaml.cs

namespace PomodoroApp.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly ViewModels.MainViewModel _viewModel;

        public MainPage(ViewModels.MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.ReloadSettings();
        }
    }
}