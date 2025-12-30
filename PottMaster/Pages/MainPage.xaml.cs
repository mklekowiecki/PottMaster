using Microsoft.Extensions.DependencyInjection;
using PottMaster.Services;
using PottMaster.ViewModels;

namespace PottMaster
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = App.Services.GetRequiredService<MainViewModel>();
        }

    }
}
