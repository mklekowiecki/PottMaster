using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Models;
using PottMaster.Resources;
using PottMaster.Services;
using System.Collections.ObjectModel;

namespace PottMaster.ViewModels;

[QueryProperty(nameof(WorkId), "workId")]
public partial class WorkDetailViewModel : ObservableObject
{
    private readonly IWorkService _workService;
    private readonly IErrorHandlingService _errorHandler;
    private readonly IAlertService _alertService;

    [ObservableProperty]
    private string? workId;

    [ObservableProperty]
    private Work? currentWork;

    [ObservableProperty]
    private ObservableCollection<WorkStatus> statuses = [];

    [ObservableProperty]
    private bool isLoading;

    public WorkDetailViewModel(IWorkService workService, IErrorHandlingService errorHandler, IAlertService alertService)
    {
        _workService = workService;
        _errorHandler = errorHandler;
        _alertService = alertService;
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
            CurrentWork!.StatusCode = Statuses.Where(s=>s.Id == CurrentWork.StatusId).Select(s=>s.Code).FirstOrDefault() ?? string.Empty;
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(LoadWorkAsync));
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
            var confirm = await _alertService.ShowConfirmationAsync(
                AppResources.ChangeStatusTitle,
                string.Format(AppResources.ConfirmChangeStatusMessage, nextStatus.Name),
                AppResources.Yes, AppResources.No);

            if (confirm)
            {
                CurrentWork.StatusId = nextStatus.Id;
                CurrentWork.StatusCode = nextStatus.Code;

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

        var confirm = await _alertService.ShowConfirmationAsync(
            AppResources.DeleteWorkTitle,
            AppResources.ConfirmDeleteWorkMessage,
            AppResources.Delete, AppResources.Cancel);

        if (confirm)
        {
            try
            {
                await _workService.DeleteWorkAsync(CurrentWork.Id);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleErrorAsync(ex, nameof(DeleteWorkAsync));
            }
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
