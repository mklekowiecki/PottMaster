using Microsoft.Extensions.DependencyInjection;
using PottMaster.Pages;
using PottMaster.Services;

namespace PottMaster
{
    public partial class AppShell : Shell
    {
        private IAuthStateService? _authStateService;
        
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute(nameof(NewWorkPage), typeof(NewWorkPage));
            Routing.RegisterRoute(nameof(WorkDetailPage), typeof(WorkDetailPage));
        }
        
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            
            if (Handler?.MauiContext?.Services != null && _authStateService == null)
            {
                _authStateService = Handler.MauiContext.Services.GetRequiredService<IAuthStateService>();
                _authStateService.AuthStateChanged += OnAuthStateChanged;
            }
        }
        
        private async void OnAuthStateChanged(object? sender, AuthStateChangedEventArgs e)
        {
            if (!e.IsAuthenticated)
            {
                await GoToAsync("//LoginPage");
            }
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            
            // Check if user is authenticated on shell navigation
            if (_authStateService != null && !_authStateService.IsAuthenticated)
            {
                await GoToAsync("//LoginPage");
            }
        }
    }
}
