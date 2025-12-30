using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using PottMaster.ViewModels;

namespace PottMaster;
public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<LoginViewModel>();
    }


}