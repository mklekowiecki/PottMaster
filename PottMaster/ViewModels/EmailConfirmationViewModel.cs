using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using PottMaster.Resources;
using Supabase;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PottMaster.ViewModels;

public partial class EmailConfirmationViewModel : ObservableObject
{
    private readonly Supabase.Client _client;
    private readonly AppLinkService _appLinkService;

    [ObservableProperty]
    private bool isLoading = true;

    [ObservableProperty]
    private string statusMessage = AppResources.ConfirmingEmail;

    [ObservableProperty]
    private bool isSuccess = false;

    public EmailConfirmationViewModel(Supabase.Client client, AppLinkService appLinkService)
    {
        _client = client;
        _appLinkService = appLinkService;
    }

    public async Task ConfirmEmailAsync()
    {
        //pottmaster://auth-callback
        
        try
        {
            var uri = _appLinkService.LastUri;
            if (uri == null)
            {
                StatusMessage = AppResources.NoConfirmationLinkReceived;
                IsLoading = false;
                return;
            }

            var query = HttpUtility.ParseQueryString(uri.Query);
            var accessToken = query["access_token"];
            var refreshToken = query["refresh_token"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                StatusMessage = AppResources.InvalidConfirmationLink;
                IsLoading = false;
                return;
            }

            var session = await _client.Auth.SetSession(accessToken, refreshToken);

            if (session?.User != null)
            {
                StatusMessage = AppResources.EmailConfirmedSuccessfully;
                IsSuccess = true;
            }
            else
            {
                StatusMessage = AppResources.FailedToConfirmEmail;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"{AppResources.Error}: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}