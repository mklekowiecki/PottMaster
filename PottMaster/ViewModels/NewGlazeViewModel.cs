using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Resources;
using PottMaster.Services;
using PottMasterLib.Models;
using PottMasterLib.Models.GlazeProperties;
using System.Collections.ObjectModel;

namespace PottMaster.ViewModels;

public partial class NewGlazeViewModel : ObservableObject
{
    private readonly IGlazeService _glazeService;
    private readonly IAuthStateService _authStateService;
    private readonly IErrorHandlingService _errorHandler;
    private readonly IAlertService _alertService;
    [ObservableProperty]
    private LocalGlaze glaze = new();

    [ObservableProperty]
    private ObservableCollection<LocalGlazeType> glazeTypes = new();

    [ObservableProperty]
    private string currentTab = "basic";

    [ObservableProperty]
    private bool isSaving;

    // Basic tab
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string? manufacturer;

    [ObservableProperty]
    private DateTime? batchDate;

    [ObservableProperty]
    private LocalGlazeType? selectedType;

    [ObservableProperty]
    private string? color;

    [ObservableProperty]
    private string? coneRating;

    [ObservableProperty]
    private string? quantity;

    [ObservableProperty]
    private bool? foodSafe;

    [ObservableProperty]
    private bool isFavorite;

    [ObservableProperty]
    private string? notes;

    // Firing tab
    [ObservableProperty]
    private int? temperatureMin;

    [ObservableProperty]
    private int? temperatureMax;

    [ObservableProperty]
    private string temperatureUnit = "C";

    [ObservableProperty]
    private string? atmosphere;

    [ObservableProperty]
    private string? curveSensitivity;

    // Appearance tab
    [ObservableProperty]
    private string? transparency;

    [ObservableProperty]
    private string? finish;

    [ObservableProperty]
    private ObservableCollection<string> selectedTextures = new();

    [ObservableProperty]
    private ObservableCollection<string> selectedEffects = new();

    // Behavior tab
    [ObservableProperty]
    private string? meltFluidity;

    [ObservableProperty]
    private string? thicknessTolerance;

    [ObservableProperty]
    private string? colorStability;

    [ObservableProperty]
    private string? repeatability;

    // Application tab
    [ObservableProperty]
    private string? form;

    [ObservableProperty]
    private ObservableCollection<string> selectedMethods = new();

    [ObservableProperty]
    private string? recommendedThickness;

    [ObservableProperty]
    private string? applicationNotes;

    // Advanced tab (Clay Compatibility, Defects, Usage)
    [ObservableProperty]
    private ObservableCollection<string> selectedClayTypes = new();

    [ObservableProperty]
    private string? clayInteraction;

    [ObservableProperty]
    private ObservableCollection<string> knownDefects = new();

    [ObservableProperty]
    private string? mitigationNotes;

    [ObservableProperty]
    private string? workType;

    [ObservableProperty]
    private string? durability;

    public NewGlazeViewModel(
        IGlazeService glazeService,
        IAuthStateService authStateService,
        IErrorHandlingService errorHandler,
        IAlertService alertService)
    {
        _glazeService = glazeService;
        _authStateService = authStateService;
        _errorHandler = errorHandler;
        _alertService = alertService;
    }

    public async Task InitializeAsync()
    {
        await LoadGlazeTypesAsync();
    }

    private async Task LoadGlazeTypesAsync()
    {
        try
        {
            var types = await _glazeService.GetGlazeTypesAsync();
            GlazeTypes = new ObservableCollection<LocalGlazeType>(types);
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(LoadGlazeTypesAsync));
        }
    }

    [RelayCommand]
    private void SelectTab(string tab)
    {
        CurrentTab = tab;
    }

    [RelayCommand]
    private void SetFoodSafe(bool? value)
    {
        FoodSafe = value;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await _alertService.ShowAlertAsync(AppResources.Error, AppResources.GlazeNameRequired ?? "Glaze name is required");
            return;
        }

        IsSaving = true;
        try
        {
            var userId = _authStateService.CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User not authenticated");
            }

            // Build the glaze object
            var glaze = new LocalGlaze
            {
                UserId = userId,
                Name = Name,
                Manufacturer = Manufacturer,
                BatchDate = BatchDate,
                TypeId = SelectedType?.Id,
                Color = Color,
                ConeRating = ConeRating,
                Quantity = Quantity,
                Notes = Notes,
                FoodSafe = FoodSafe,
                IsFavorite = IsFavorite,
                Properties = BuildProperties()
            };

            await _glazeService.CreateGlazeAsync(glaze);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(SaveAsync));
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private GlazePropertiesModel BuildProperties()
    {
        return new GlazePropertiesModel
        {
            Firing = new FiringProperties
            {
                TemperatureMin = TemperatureMin,
                TemperatureMax = TemperatureMax,
                TemperatureUnit = TemperatureUnit,
                Atmosphere = Atmosphere,
                CurveSensitivity = CurveSensitivity
            },
            Appearance = new AppearanceProperties
            {
                Transparency = Transparency,
                Finish = Finish,
                Texture = SelectedTextures.ToList(),
                SpecialEffects = SelectedEffects.ToList()
            },
            Behavior = new BehaviorProperties
            {
                MeltFluidity = MeltFluidity,
                ThicknessTolerance = ThicknessTolerance,
                ColorStability = ColorStability,
                Repeatability = Repeatability
            },
            Application = new ApplicationProperties
            {
                Form = Form,
                Methods = SelectedMethods.ToList(),
                RecommendedThickness = RecommendedThickness,
                ApplicationNotes = ApplicationNotes
            },
            ClayCompatibility = new ClayCompatibilityProperties
            {
                BestSuited = SelectedClayTypes.ToList(),
                Interaction = ClayInteraction
            },
            Defects = new DefectsProperties
            {
                KnownIssues = KnownDefects.ToList(),
                MitigationNotes = MitigationNotes
            },
            Usage = new UsageProperties
            {
                WorkType = WorkType,
                Durability = Durability
            }
        };
    }
}
