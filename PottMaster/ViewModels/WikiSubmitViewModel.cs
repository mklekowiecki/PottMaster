using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using PottMasterLib.Models;
using System.Collections.ObjectModel;

namespace PottMaster.ViewModels;

public partial class WikiSubmitViewModel : ObservableObject
{
    private readonly IWikiService _wikiService;
    private readonly IAlertService _alertService;
    private readonly IErrorHandlingService _errorHandler;
    private readonly IAuthStateService _authStateService;

    [ObservableProperty]
    private ObservableCollection<WikiMaterialType> materialTypes = new();

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private WikiMaterialType? selectedType;

    [ObservableProperty]
    private string properties = string.Empty;

    [ObservableProperty]
    private string manufacturer = string.Empty;

    [ObservableProperty]
    private bool isSubmitting;

    public WikiSubmitViewModel(
        IWikiService wikiService,
        IAlertService alertService,
        IErrorHandlingService errorHandler,
        IAuthStateService authStateService)
    {
        _wikiService = wikiService;
        _alertService = alertService;
        _errorHandler = errorHandler;
        _authStateService = authStateService;
    }

    public async Task InitializeAsync()
    {
        await LoadMaterialTypesAsync();
    }

    [RelayCommand]
    private async Task LoadMaterialTypesAsync()
    {
        try
        {
            var types = await _wikiService.GetMaterialTypesAsync();
            MaterialTypes = new ObservableCollection<WikiMaterialType>(types);
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(LoadMaterialTypesAsync));
        }
    }

    [RelayCommand]
    private async Task SubmitMaterialAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) || SelectedType == null)
        {
            await _alertService.ShowAlertAsync("B??d", "Nazwa i typ s? wymagane.");
            return;
        }

        IsSubmitting = true;
        try
        {
            var material = new WikiMaterial
            {
                Id = Guid.NewGuid(), // Generate new Guid
                Name = Name,
                Description = Description ?? "",
                TypeId = SelectedType.Id,
                Manufacturer = Manufacturer ?? "",
                Properties = Properties ?? "",
                TrustLevel = "unverified",
                SubmittedBy = Guid.Parse(_authStateService.CurrentUserId ?? Guid.Empty.ToString()),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _wikiService.SubmitMaterialAsync(material);
            await _alertService.ShowAlertAsync("Sukces", "Materia? zosta? przes?any do weryfikacji.");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(SubmitMaterialAsync));
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}