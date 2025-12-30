using Microsoft.Extensions.DependencyInjection;
using PottMaster.Services;

namespace PottMaster
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
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
