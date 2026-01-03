using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using PottMaster.Resources;

namespace PottMaster.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IAuthStateService _authStateService;
    private readonly IAlertService _alertService;
    private readonly IErrorHandlingService _errorHandler;

    [ObservableProperty]
    private string userEmail = string.Empty;

    [ObservableProperty]
    private string userInitials = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string appVersion = string.Empty;

    public ProfileViewModel(
        IAuthService authService,
        IAuthStateService authStateService,
        IAlertService alertService,
        IErrorHandlingService errorHandler)
    {
        _authService = authService;
        _authStateService = authStateService;
        _alertService = alertService;
        _errorHandler = errorHandler;
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
            UserEmail = _authStateService.CurrentUserEmail ?? string.Empty;
            UserInitials = _authStateService.UserInitials ?? string.Empty;
            
            // Get app version
            AppVersion = $"v{AppInfo.VersionString}";
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(InitializeAsync));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await _alertService.ShowConfirmationAsync(
            "Logout",
            "Are you sure you want to logout?");

        if (!confirm)
            return;

        IsLoading = true;

        try
        {
            await _authService.SignOutAsync();
            await _authStateService.ClearAuthenticationAsync();
            
            // Navigate to login
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(LogoutAsync));
            await _alertService.ShowAlertAsync(
                AppResources.Error,
                ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
