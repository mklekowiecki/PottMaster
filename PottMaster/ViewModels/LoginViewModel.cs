using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using PottMaster.Resources;
using Microsoft.Maui.Storage;

namespace PottMaster.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IAuthStateService _authStateService;
    private readonly IErrorHandlingService _errorHandler;
    private readonly IBiometricService _biometricService;

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

    public bool IsBiometricAvailable { get; private set; }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    
    public bool IsLoginEnabled => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !IsBusy;

    public LoginViewModel(IAuthService authService, IAuthStateService authStateService, IErrorHandlingService errorHandler, IBiometricService biometricService)
    {
        _authService = authService;
        _authStateService = authStateService;
        _errorHandler = errorHandler;
        _biometricService = biometricService;
        IsBiometricAvailable = _biometricService.IsBiometricAvailable();
#if DEBUG || DEBUG_REMOTE
        Email = "m.klekowiecki@gmail.com";
        Password = "Tiamat1234!";
#endif

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
            
            // Store credentials for biometric login
            await SecureStorage.SetAsync("email", Email);
            await SecureStorage.SetAsync("password", Password);
            
            await Shell.Current.GoToAsync("//main");
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

    [RelayCommand]
    private async Task FingerprintLogin()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        
        try
        {
            var success = await _biometricService.AuthenticateBiometricAsync(AppResources.FingerPrintLogin, AppResources.FingerPrintLoginMsg);
            if (!success)
            {
                ErrorMessage =  AppResources.FingerPrintLoginFailed;
                return;
            }
            
            var email = await SecureStorage.GetAsync("email");
            var password = await SecureStorage.GetAsync("password");
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ErrorMessage = AppResources.NoFingerPrintDataSaved;
                return;
            }
            
            Email = email;
            Password = password;
            await Login();
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(FingerprintLogin));
            ErrorMessage = _errorHandler.GetUserFriendlyError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}