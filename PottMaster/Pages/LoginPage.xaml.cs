using Microsoft.Maui.Controls;
using PottMaster.ViewModels;

namespace PottMaster;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}