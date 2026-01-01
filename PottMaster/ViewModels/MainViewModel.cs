using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Models;
using PottMaster.Services;
using System.Collections.ObjectModel;

namespace PottMaster.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IWorkService _workService;
    private readonly IAuthService _authService;
    private readonly IDbService _dbService;

    [ObservableProperty]
    private ObservableCollection<Work> works = [];

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    public MainViewModel(IWorkService workService, IAuthService authService, IDbService dbService)
    {
        _workService = workService;
        _authService = authService;
        _dbService = dbService;
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
            var user = await _authService.GetCurrentUserAsync();
            if (user?.Id != null)
            {
                var worksList = await _workService.GetUserWorksAsync(user.Id);
                Works = new ObservableCollection<Work>(worksList);
                IsEmpty = Works.Count == 0;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load works: {ex.Message}", "OK");
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
        await Shell.Current.GoToAsync($"WorkDetailPage?workId={work.Id}");
    }
}