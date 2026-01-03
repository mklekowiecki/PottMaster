using Microsoft.Extensions.DependencyInjection;
using PottMaster.Pages;
using PottMaster.Services;
using PottMaster.ViewModels;

namespace PottMaster
{
    public partial class AppShell : Shell
    {
        private IAuthStateService? _authStateService;
        private SyncStatusViewModel? _syncStatusViewModel;
        
        public AppShell()
        {
            InitializeComponent();
            
            // Register routes for detail pages (modal navigation)
            Routing.RegisterRoute(nameof(NewWorkPage), typeof(NewWorkPage));
            Routing.RegisterRoute(nameof(WorkDetailPage), typeof(WorkDetailPage));
            Routing.RegisterRoute(nameof(EmailConfirmationPage), typeof(EmailConfirmationPage));
        }
        
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            
            if (Handler?.MauiContext?.Services != null)
            {
                if (_authStateService == null)
                {
                    _authStateService = Handler.MauiContext.Services.GetRequiredService<IAuthStateService>();
                    _authStateService.AuthStateChanged += OnAuthStateChanged;
                    
                    // Set initial tab bar visibility
                    UpdateTabBarVisibility(_authStateService.IsAuthenticated);
                }
                
                if (_syncStatusViewModel == null)
                {
                    _syncStatusViewModel = Handler.MauiContext.Services.GetRequiredService<SyncStatusViewModel>();
                    SyncIndicator.BindingContext = _syncStatusViewModel;
                }
            }
        }
        
        private async void OnAuthStateChanged(object? sender, AuthStateChangedEventArgs e)
        {
            UpdateTabBarVisibility(e.IsAuthenticated);
            
            if (!e.IsAuthenticated)
            {
                await GoToAsync("//LoginPage");
            }
            else
            {
                await GoToAsync("//main");
            }
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            
            // Check if user is authenticated on shell navigation
            if (_authStateService != null)
            {
                UpdateTabBarVisibility(_authStateService.IsAuthenticated);
                
                if (!_authStateService.IsAuthenticated)
                {
                    var currentRoute = Shell.Current.CurrentState.Location.OriginalString;
                    if (!currentRoute.Contains("LoginPage") && !currentRoute.Contains("SignupPage") && !currentRoute.Contains("EmailConfirmationPage"))
                    {
                        await GoToAsync("//LoginPage");
                    }
                }
            }
        }
        
        private void UpdateTabBarVisibility(bool isAuthenticated)
        {
            // Hide/show the main tab bar based on authentication status
            var tabBar = this.FindByName<TabBar>("MainTabBar");
            if (tabBar != null)
            {
                tabBar.IsVisible = isAuthenticated;
            }
        }
    }
}
