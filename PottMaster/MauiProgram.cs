namespace PottMaster
{
	using Microsoft.Extensions.Logging;
	using PottMaster.Pages;
	using PottMaster.Services;
	using PottMaster.ViewModels;
	using PottMaster.Repositories;
	using System.Globalization;

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
			
			// Infrastructure Services (Singleton - shared across app lifetime)
			builder.Services.AddSingleton<IDbService, DbService>();
			builder.Services.AddSingleton<IImageService, ImageService>();
			builder.Services.AddSingleton<ISyncService, SyncService>();
			builder.Services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
			
			// Authentication Services (Singleton - maintains auth state)
			builder.Services.AddSingleton<IAuthService, AuthService>();
			builder.Services.AddSingleton<IAuthStateService, AuthStateService>();
			
			// Repository Pattern (Scoped - per operation context)
			builder.Services.AddScoped<IWorkRepository, LocalWorkRepository>();
			
			// Business Logic Services (Scoped - user-specific operations)
			builder.Services.AddScoped<IWorkService, WorkService>();
			
			// ViewModels (Transient - new instance per navigation)
			builder.Services.AddTransient<LoginViewModel>();
			builder.Services.AddTransient<SignupViewModel>();
			builder.Services.AddTransient<MainViewModel>();
			builder.Services.AddTransient<NewWorkViewModel>();
			builder.Services.AddTransient<WorkDetailViewModel>();
			
			// Pages (Transient - new instance per navigation)
			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<NewWorkPage>();
			builder.Services.AddTransient<LoginPage>();
			builder.Services.AddTransient<SignupPage>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			// Set culture but respect device settings
			var deviceCulture = CultureInfo.CurrentUICulture;
			var culture = new CultureInfo("pl"); 

			// Only override if device is not already using Polish or English
			if (deviceCulture.TwoLetterISOLanguageName != "pl" && deviceCulture.TwoLetterISOLanguageName != "en")
			{
				Thread.CurrentThread.CurrentCulture = culture;
				Thread.CurrentThread.CurrentUICulture = culture;

			}

#if DEBUG
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
#endif

			var mauiApp = builder.Build();
			
			// Initialize auth state on startup
			var authStateService = mauiApp.Services.GetRequiredService<IAuthStateService>();
			_ = Task.Run(async () => await authStateService.InitializeAsync());
			
			return mauiApp;
		}
	}
}
