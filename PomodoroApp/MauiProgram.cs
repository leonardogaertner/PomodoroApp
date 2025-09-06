// PomodoroApp/MauiProgram.cs

using Microsoft.Extensions.Logging;
using PomodoroApp.Views;
using PomodoroApp.ViewModels;

namespace PomodoroApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar Views e ViewModels para injeção de dependência
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<Setup>();
            builder.Services.AddSingleton<SetupViewModel>();
            builder.Services.AddSingleton<Knowledge>();
            builder.Services.AddSingleton<KnowledgeViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}