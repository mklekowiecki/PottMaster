namespace PottMaster
{
	using Microsoft.Extensions.Logging;
	using PottMaster.Services;
	using PottMaster.ViewModels;
	using System.Globalization;

	/// <summary>
	/// Defines the <see cref="MauiProgram" />
	/// </summary>
	public static class MauiProgram
	{
		/// <summary>
		/// The CreateMauiApp
		/// </summary>
		/// <returns>The <see cref="MauiApp"/></returns>
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
			CultureInfo culture = new CultureInfo("pl"); 
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;

			var mauiApp = builder.Build();
			App.SetServiceProvider(mauiApp.Services);
			return mauiApp;
		}
	}
}
