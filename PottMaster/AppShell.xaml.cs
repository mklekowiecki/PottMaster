using Microsoft.Extensions.DependencyInjection;
using PottMaster.Pages;
using PottMaster.Services;

namespace PottMaster
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute(nameof(NewWorkPage), typeof(NewWorkPage));
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (!App.Services.GetRequiredService<IAuthService>().IsLoggedIn)
            {
                await GoToAsync("//LoginPage");
            }
        }
    }
}
