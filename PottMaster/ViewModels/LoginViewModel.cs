using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using PottMaster.Resources;

namespace PottMaster.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IAuthStateService _authStateService;
    private readonly IErrorHandlingService _errorHandler;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoginEnabled))]
    private string email = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoginEnabled))]
    private string password = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoginEnabled))]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    
    public bool IsLoginEnabled => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !IsBusy;

    public LoginViewModel(IAuthService authService, IAuthStateService authStateService, IErrorHandlingService errorHandler)
    {
        _authService = authService;
        _authStateService = authStateService;
        _errorHandler = errorHandler;
    }

    [RelayCommand(CanExecute = nameof(IsLoginEnabled))]
    private async Task Login()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        
        try
        {
            var signInResult = await _authService.SignInAsync(Email, Password);
            if (signInResult.Result != AuthResult.Success)
            {
                ErrorMessage = AppResources.LoginFailed;
                return;
            }
            
            var profileResult = await _authService.GetUserProfileAsync();
            if (profileResult.Result != AuthResult.Success || profileResult.Data == null)
            {
                ErrorMessage = AppResources.ErrorLoadingProfile;
                return;
            }
            
            // Update auth state
            await _authStateService.SetAuthenticatedAsync(
                profileResult.Data.Id, 
                profileResult.Data.Email, 
                profileResult.Data.Initials);
            
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(Login));
            ErrorMessage = _errorHandler.GetUserFriendlyError(ex);
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