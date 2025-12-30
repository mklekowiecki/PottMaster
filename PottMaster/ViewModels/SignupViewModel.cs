using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using System.ComponentModel;

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
        try
        {
            await _authService.SignUpAsync(Email, Password);
            await Application.Current.MainPage.DisplayAlert("Success", "Account created. Please log in.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task NavigateToLogin()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}