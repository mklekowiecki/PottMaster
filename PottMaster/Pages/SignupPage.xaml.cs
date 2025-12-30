using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using PottMaster.ViewModels;

namespace PottMaster;
public partial class SignupPage : ContentPage
{
    public SignupPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<SignupViewModel>();
    }


}