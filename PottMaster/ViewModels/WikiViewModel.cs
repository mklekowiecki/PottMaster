using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Resources;
using PottMaster.Services;
using PottMasterLib.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace PottMaster.ViewModels;

public partial class WikiViewModel : ObservableObject
{
    private readonly IWikiService _wikiService;
    private readonly IAlertService _alertService;
    private readonly IErrorHandlingService _errorHandler;

    [ObservableProperty]
    private ObservableCollection<WikiMaterial> materials = new();

    [ObservableProperty]
    private ObservableCollection<WikiMaterialType> materialTypes = new();

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private WikiMaterialType? selectedType;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    public WikiViewModel(
        IWikiService wikiService,
        IAlertService alertService,
        IErrorHandlingService errorHandler)
    {
        _wikiService = wikiService;
        _alertService = alertService;
        _errorHandler = errorHandler;
    }

    public async Task InitializeAsync()
    {
        await LoadMaterialTypesAsync();
        await SearchMaterialsAsync();
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
    private async Task SearchMaterialsAsync()
    {
        IsLoading = true;
        try
        {
            List<WikiMaterial> results;
            if (SelectedType != null)
            {
                results = await _wikiService.GetMaterialsByTypeAsync(SelectedType.Id);
            }
            else if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                results = await _wikiService.SearchMaterialsAsync(SearchQuery);
            }
            else
            {
                results = await _wikiService.GetAllMaterialsAsync();
            }

            Materials = new ObservableCollection<WikiMaterial>(results);
            IsEmpty = Materials.Count == 0;
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(SearchMaterialsAsync));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ViewMaterialDetailsAsync(WikiMaterial material)
    {
        if (material?.Id != Guid.Empty)
        {
            // Navigate to detail page
            var parameters = new Dictionary<string, object>
            {
                { "materialId", material.Id.ToString() }
            };
            await Shell.Current.GoToAsync("WikiDetailPage", parameters);
        }
    }

    [RelayCommand]
    private async Task AddNewMaterialAsync()
    {
        await Shell.Current.GoToAsync("WikiSubmitPage");
    }

    partial void OnSearchQueryChanged(string oldValue, string newValue)
    {
        SearchMaterialsCommand.Execute(null);
    }

    partial void OnSelectedTypeChanged(WikiMaterialType? oldValue, WikiMaterialType? newValue)
    {
        SearchMaterialsCommand.Execute(null);
    }
}