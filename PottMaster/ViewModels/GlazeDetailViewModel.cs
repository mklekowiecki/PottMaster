using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Pages;
using PottMaster.Resources;
using PottMaster.Services;
using PottMasterLib.Models;

namespace PottMaster.ViewModels;

[QueryProperty(nameof(GlazeId), "glazeId")]
public partial class GlazeDetailViewModel : ObservableObject
{
    private readonly IGlazeService _glazeService;
    private readonly IErrorHandlingService _errorHandler;
    private readonly IAlertService _alertService;

    [ObservableProperty]
    private string? glazeId;

    [ObservableProperty]
    private LocalGlaze? glaze;

    [ObservableProperty]
    private bool isLoading;

    public GlazeDetailViewModel(
        IGlazeService glazeService, 
        IErrorHandlingService errorHandler, 
        IAlertService alertService)
    {
        _glazeService = glazeService;
        _errorHandler = errorHandler;
        _alertService = alertService;
    }

    partial void OnGlazeIdChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            Task.Run(async () => await LoadGlazeAsync());
        }
    }

    private async Task LoadGlazeAsync()
    {
        if (string.IsNullOrEmpty(GlazeId))
            return;

        IsLoading = true;
        try
        {
            Glaze = await _glazeService.GetGlazeByIdAsync(GlazeId);
            
            if (Glaze != null && Glaze.TypeId.HasValue)
            {
                var glazeTypes = await _glazeService.GetGlazeTypesAsync();
                var type = glazeTypes.FirstOrDefault(t => t.Id == Glaze.TypeId.Value);
                if (type != null)
                {
                    Glaze.TypeName = type.Name;
                }
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(LoadGlazeAsync));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (Glaze == null)
            return;

        try
        {
            Glaze.IsFavorite = !Glaze.IsFavorite;
            await _glazeService.UpdateGlazeAsync(Glaze);
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(ToggleFavoriteAsync));
        }
    }

    [RelayCommand]
    private async Task EditAsync()
    {
        if (Glaze == null)
            return;

        await Shell.Current.GoToAsync($"{nameof(NewGlazePage)}?glazeId={Glaze.Id}");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (Glaze == null)
            return;

        var confirm = await _alertService.ShowConfirmationAsync(
            AppResources.DeleteGlaze,
            string.Format("Czy na pewno chcesz usun?? {0}?", Glaze.Name),
            AppResources.Delete,
            AppResources.Cancel);

        if (confirm)
        {
            try
            {
                await _glazeService.DeleteGlazeAsync(Glaze.Id);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleErrorAsync(ex, nameof(DeleteAsync));
            }
        }
    }
}
