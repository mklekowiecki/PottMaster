using Microsoft.Extensions.Logging;
using PottMaster.Services;
using PottMaster.ViewModels;

namespace PottMaster
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
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddTransient<LoginViewModel>();
builder.Services.AddTransient<SignupViewModel>();
builder.Services.AddTransient<MainViewModel>();


#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var mauiApp = builder.Build();
            App.SetServiceProvider(mauiApp.Services);
            return mauiApp;
        }
    }
}
