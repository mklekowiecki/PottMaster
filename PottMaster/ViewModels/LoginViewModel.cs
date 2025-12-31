using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using PottMaster.Resources; // Add for localization

namespace PottMaster.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoginEnabled))]
    private string email = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoginEnabled))]
    private string password = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoginEnabled))]
    private bool isBusy;
    
    public bool IsLoginEnabled => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !IsBusy;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand(CanExecute = nameof(IsLoginEnabled))]
    private async Task Login()
    {
        IsBusy = true;
        try
        {
            var signInResult = await _authService.SignInAsync(Email, Password);
            if (signInResult.Result != AuthResult.Success)
            {
                await Application.Current.MainPage.DisplayAlert(Resource.Error, signInResult.ErrorMessage ?? Resource.LoginFailed, Resource.Ok);
                return;
            }
            var profileResult = await _authService.GetUserProfileAsync();
            if (profileResult.Result != AuthResult.Success || profileResult.Data == null)
            {
                await Application.Current.MainPage.DisplayAlert(Resource.Error, profileResult.ErrorMessage ?? Resource.ErrorLoadingProfile, Resource.Ok);
                return;
            }
            Preferences.Default.Set("UserInitials", profileResult.Data.Initials);
            await Shell.Current.GoToAsync("//MainPage");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Signup()
    {
        await Shell.Current.GoToAsync("//SignupPage");
    }
}