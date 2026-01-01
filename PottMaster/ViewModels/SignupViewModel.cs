using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using System.ComponentModel;
using PottMaster.Resources;

namespace PottMaster.ViewModels;

public partial class SignupViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IErrorHandlingService _errorHandler;
    private readonly IAlertService _alertService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSignupEnabled))]
    private string email = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSignupEnabled))]
    private string password = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSignupEnabled))]
    private bool isBusy;

    public bool IsSignupEnabled => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !IsBusy;

    public SignupViewModel(IAuthService authService, IErrorHandlingService errorHandler, IAlertService alertService)
    {
        _authService = authService;
        _errorHandler = errorHandler;
        _alertService = alertService;
    }

    [RelayCommand(CanExecute = nameof(IsSignupEnabled))]
    private async Task SignupAsync()
    {
        IsBusy = true;
        try
        {
            var signupResult = await _authService.SignUpAsync(Email, Password);
            if (signupResult.Result != AuthResult.Success)
            {
                await _alertService.ShowAlertAsync(AppResources.Error, AppResources.SignUpError);
                return;
            }
            await _alertService.ShowAlertAsync(AppResources.Success, AppResources.AccountCreatedPleaseLogin);
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(SignupAsync));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToLogin()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}