using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Models;
using PottMaster.Services;
using System.Collections.ObjectModel;

namespace PottMaster.ViewModels;

[QueryProperty(nameof(WorkId), "workId")]
public partial class WorkDetailViewModel : ObservableObject
{
    private readonly IWorkService _workService;

    [ObservableProperty]
    private string? workId;

    [ObservableProperty]
    private Work? currentWork;

    [ObservableProperty]
    private ObservableCollection<WorkStatus> statuses = [];

    [ObservableProperty]
    private bool isLoading;

    public WorkDetailViewModel(IWorkService workService)
    {
        _workService = workService;
    }

    partial void OnWorkIdChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            Task.Run(async () => await LoadWorkAsync());
        }
    }

    private async Task LoadWorkAsync()
    {
        if (string.IsNullOrEmpty(WorkId))
            return;

        IsLoading = true;
        try
        {
            CurrentWork = await _workService.GetWorkByIdAsync(WorkId);
            var statusesList = await _workService.GetStatusesAsync();
            Statuses = new ObservableCollection<WorkStatus>(statusesList);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load work: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ChangeStatusAsync()
    {
        if (CurrentWork == null)
            return;

        var currentStatusIndex = Statuses.ToList().FindIndex(s => s.Id == CurrentWork.StatusId);
        if (currentStatusIndex >= 0 && currentStatusIndex < Statuses.Count - 1)
        {
            var nextStatus = Statuses[currentStatusIndex + 1];
            var confirm = await Shell.Current.DisplayAlert(
                "Change Status",
                $"Move work to '{nextStatus.Name}' status?",
                "Yes", "No");

            if (confirm)
            {
                CurrentWork.StatusId = nextStatus.Id;
                CurrentWork.StatusName = nextStatus.Name;

                if (nextStatus.Id == 3)
                {
                    CurrentWork.DryingCompletedAt = DateTime.UtcNow;
                }

                await _workService.UpdateWorkAsync(CurrentWork);
                await LoadWorkAsync();
            }
        }
    }

    [RelayCommand]
    private async Task DeleteWorkAsync()
    {
        if (CurrentWork == null)
            return;

        var confirm = await Shell.Current.DisplayAlert(
            "Delete Work",
            "Are you sure you want to delete this work?",
            "Delete", "Cancel");

        if (confirm)
        {
            await _workService.DeleteWorkAsync(CurrentWork.Id);
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
