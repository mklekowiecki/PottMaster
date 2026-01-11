using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using PottMasterLib.Models;
using System.Threading.Tasks;

namespace PottMaster.ViewModels;

public partial class WikiDetailViewModel : ObservableObject
{
    private readonly IWikiService _wikiService;
    private readonly IAlertService _alertService;
    private readonly IErrorHandlingService _errorHandler;

    [ObservableProperty]
    private WikiMaterial? material;

    [ObservableProperty]
    private bool isLoading;

    private Guid _materialId;

    public string? MaterialId
    {
        get => _materialId.ToString();
        set
        {
            if (Guid.TryParse(value, out var guid))
            {
                _materialId = guid;
                Task.Run(() => LoadMaterialAsync(guid));
            }
        }
    }

    public WikiDetailViewModel(
        IWikiService wikiService,
        IAlertService alertService,
        IErrorHandlingService errorHandler)
    {
        _wikiService = wikiService;
        _alertService = alertService;
        _errorHandler = errorHandler;
    }

    private async Task LoadMaterialAsync(Guid id)
    {
        IsLoading = true;
        try
        {
            Material = await _wikiService.GetMaterialByIdAsync(id);
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(LoadMaterialAsync));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task VerifyAsync()
    {
        if (Material == null)
            return;

        var confirm = await _alertService.ShowConfirmationAsync("Zweryfikuj", "Czy potwierdzasz weryfikacj? tego materia?u?", "Tak", "Nie");
        if (confirm)
        {
            try
            {
                await _wikiService.VerifyMaterialAsync(Material.Id);
                Material.TrustLevel = "verified";
                OnPropertyChanged(nameof(Material));
                await _alertService.ShowAlertAsync("Sukces", "Materia? zosta? zweryfikowany.");
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleErrorAsync(ex, nameof(VerifyAsync));
            }
        }
    }
}