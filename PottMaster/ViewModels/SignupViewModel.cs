using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using System.ComponentModel;
using PottMaster.Resources; // Add for localization

namespace PottMaster.ViewModels;

public partial class SignupViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSignupEnabled))]
    private string email = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSignupEnabled))]
    private string password = string.Empty;

    public bool IsSignupEnabled => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

    public SignupViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand(CanExecute = nameof(IsSignupEnabled))]
    private async Task SignupAsync()
    {
        var signupResult = await _authService.SignUpAsync(Email, Password);
        if (signupResult.Result != AuthResult.Success)
        {
            await Application.Current!.Windows[0].Page!.DisplayAlert(AppResources.Error, AppResources.SignUpError, AppResources.Ok);
            return;
        }
        await Application.Current!.Windows[0].Page!.DisplayAlert(AppResources.Success, AppResources.AccountCreatedPleaseLogin, AppResources.Ok);
        await Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    private async Task NavigateToLogin()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}