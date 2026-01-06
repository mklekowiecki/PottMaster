using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Resources;
using PottMaster.Services;
using PottMasterLib.Models;
using PottMasterLib.Models.GlazeProperties;
using System.Collections.ObjectModel;

namespace PottMaster.ViewModels;

public partial class GlazeInventoryViewModel : ObservableObject
{
    private readonly IGlazeService _glazeService;
    private readonly IAuthStateService _authStateService;
    private readonly IErrorHandlingService _errorHandler;
    private readonly IAlertService _alertService;
    [ObservableProperty]
    private ObservableCollection<LocalGlaze> glazes = new();

    [ObservableProperty]
    private ObservableCollection<LocalGlaze> filteredGlazes = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool showFavoritesOnly;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    public GlazeInventoryViewModel(
        IGlazeService glazeService,
        IAuthStateService authStateService,
        IErrorHandlingService errorHandler,
        IAlertService alertService)
    {
        _glazeService = glazeService;
        _authStateService = authStateService;
        _errorHandler = errorHandler;
        _alertService = alertService;

        _authStateService.AuthStateChanged += OnAuthStateChanged;
    }

    private async void OnAuthStateChanged(object? sender, AuthStateChangedEventArgs e)
    {
        if (e.IsAuthenticated)
        {
            await LoadGlazesAsync();
        }
        else
        {
            Glazes.Clear();
            FilteredGlazes.Clear();
            IsEmpty = true;
        }
    }

    public async Task InitializeAsync()
    {
        await LoadGlazesAsync();
    }

    [RelayCommand]
    private async Task LoadGlazesAsync()
    {
        IsLoading = true;
        try
        {
            var userId = _authStateService.CurrentUserId;
            if (!string.IsNullOrEmpty(userId))
            {
                var glazeList = await _glazeService.GetUserGlazesAsync(userId);
                var glazeTypes = await _glazeService.GetGlazeTypesAsync();
                
                // Populate TypeName for each glaze
                foreach (var glaze in glazeList)
                {
                    if (glaze.TypeId.HasValue)
                    {
                        var type = glazeTypes.FirstOrDefault(t => t.Id == glaze.TypeId.Value);
                        if (type != null)
                        {
                            glaze.TypeName = type.Name;
                        }
                    }
                }
                
                Glazes = new ObservableCollection<LocalGlaze>(glazeList);
                ApplyFilters();
            }
            else
            {
                Glazes.Clear();
                FilteredGlazes.Clear();
                IsEmpty = true;
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(LoadGlazesAsync));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddGlazeAsync()
    {
        await Shell.Current.GoToAsync("NewGlazePage");
    }

    [RelayCommand]
    private async Task ViewGlazeDetailsAsync(LocalGlaze glaze)
    {
        if (glaze?.Id != null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "glazeId", glaze.Id }
            };
            await Shell.Current.GoToAsync("GlazeDetailPage", parameters);
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(LocalGlaze glaze)
    {
        try
        {
            glaze.IsFavorite = !glaze.IsFavorite;
            await _glazeService.UpdateGlazeAsync(glaze);
            ApplyFilters();
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(ToggleFavoriteAsync));
        }
    }

    [RelayCommand]
    private async Task DeleteGlazeAsync(LocalGlaze glaze)
    {
        try
        {
            bool confirm = await _alertService.ShowConfirmationAsync(
                AppResources.DeleteGlaze ?? "Delete Glaze",
                string.Format(AppResources.ConfirmDeleteGlaze ?? "Are you sure you want to delete {0}?", glaze.Name),
                AppResources.Delete ?? "Delete",
                AppResources.Cancel ?? "Cancel");

            if (confirm)
            {
                await _glazeService.DeleteGlazeAsync(glaze.Id);
                await LoadGlazesAsync();
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(DeleteGlazeAsync));
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnShowFavoritesOnlyChanged(bool value)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = Glazes.AsEnumerable();

        if (ShowFavoritesOnly)
        {
            filtered = filtered.Where(g => g.IsFavorite);
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLower();
            filtered = filtered.Where(g =>
                g.Name.ToLower().Contains(searchLower) ||
                (g.Manufacturer?.ToLower().Contains(searchLower) ?? false) ||
                (g.Color?.ToLower().Contains(searchLower) ?? false) ||
                (g.ConeRating?.ToLower().Contains(searchLower) ?? false));
        }

        FilteredGlazes = new ObservableCollection<LocalGlaze>(filtered);
        IsEmpty = FilteredGlazes.Count == 0;
    }
}
