using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using PottMaster.Resources;
using System.Collections.ObjectModel;
using PottMasterLib.Services;
using PottMasterLib.Models;

namespace PottMaster.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IWorkService _workService;
    private readonly IAuthStateService _authStateService;
    private readonly IErrorHandlingService _errorHandler;
    private readonly IDbService _dbService;

    [ObservableProperty]
    private ObservableCollection<Work> works = [];

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    public MainViewModel(
        IWorkService workService, 
        IAuthStateService authStateService,
        IErrorHandlingService errorHandler,
        IDbService dbService)
    {
        _workService = workService;
        _authStateService = authStateService;
        _errorHandler = errorHandler;
        _dbService = dbService;
        
        // Subscribe to auth state changes
        _authStateService.AuthStateChanged += OnAuthStateChanged;
    }
    
    private async void OnAuthStateChanged(object? sender, AuthStateChangedEventArgs e)
    {
        if (e.IsAuthenticated)
        {
            await LoadWorksAsync();
        }
        else
        {
            Works.Clear();
            IsEmpty = true;
        }
    }

    public async Task InitializeAsync()
    {
        await _dbService.InitializeAsync();
        await LoadWorksAsync();
    }

    [RelayCommand]
    private async Task LoadWorksAsync()
    {
        IsLoading = true;
        try
        {
            var userId = _authStateService.CurrentUserId;
            if (!string.IsNullOrEmpty(userId))
            {
                var worksList = await _workService.GetUserWorksAsync(userId);
                Works = new ObservableCollection<Work>(worksList);
                IsEmpty = Works.Count == 0;
            }
            else
            {
                Works.Clear();
                IsEmpty = true;
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(LoadWorksAsync));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddWorkAsync()
    {
        await Shell.Current.GoToAsync("NewWorkPage");
    }

    [RelayCommand]
    private async Task ViewWorkDetailsAsync(Work work)
    {
        if (work?.Id != null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "workId", work.Id }
            };
            await Shell.Current.GoToAsync("WorkDetailPage", parameters);
        }
    }
}