using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;

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
            await _authService.SignInAsync(Email, Password);
            var profile = await _authService.GetUserProfileAsync();
            Preferences.Default.Set("UserInitials", profile.Initials);
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
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