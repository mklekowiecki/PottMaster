using Microsoft.Maui.Controls;
using PottMaster.Pages;
using PottMaster.Services;

namespace PottMaster
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
        
        protected override async void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);
        
            if (uri.Host == "auth-callback")
            {
                // Store the URI in the service
                var appLinkService = Application.Current?.Handler?.MauiContext?.Services?.GetService<AppLinkService>();
                if (appLinkService != null)
                {
                    appLinkService.LastUri = uri;
                }
        
                // Delay navigation to allow shell to initialize
                await Task.Delay(500);
                // Navigate to the confirmation page
                await Shell.Current.GoToAsync(nameof(EmailConfirmationPage));
            }
        }
                 
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}