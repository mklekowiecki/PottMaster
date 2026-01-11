using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Resources;
using PottMaster.Services;
using PottMasterLib.Models;
using PottMasterLib.Models.GlazeProperties;
using System.Collections.ObjectModel;

namespace PottMaster.ViewModels;

[QueryProperty(nameof(GlazeId), "glazeId")]
public partial class NewGlazeViewModel : ObservableObject
{
    private readonly IGlazeService _glazeService;
    private readonly IAuthStateService _authStateService;
    private readonly IErrorHandlingService _errorHandler;
    private readonly IAlertService _alertService;

    [ObservableProperty]
    private string? glazeId;

    [ObservableProperty]
    private LocalGlaze glaze = new();

    [ObservableProperty]
    private ObservableCollection<LocalGlazeType> glazeTypes = new();

    [ObservableProperty]
    private string currentTab = "basic";

    [ObservableProperty]
    private bool isSaving;

    [ObservableProperty]
    private bool isEditMode;

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
    private string? foodSafe;

    [ObservableProperty]
    private ObservableCollection<string> foodSafeOptions = new() { AppResources.Yes, AppResources.No, AppResources.NotSpecified };

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

    [ObservableProperty]
    private ObservableCollection<string> temperatureUnits = new() { "C", "F" };

    [ObservableProperty]
    private ObservableCollection<string> atmospheres = new() { AppResources.Oxidation, AppResources.Reduction };

    [ObservableProperty]
    private ObservableCollection<string> curveSensitivities = new() { AppResources.Low, AppResources.Medium, AppResources.High };

    [ObservableProperty]
    private ObservableCollection<string> transparencies = new() { AppResources.Transparent, AppResources.SemiTransparent, AppResources.Opaque };

    [ObservableProperty]
    private ObservableCollection<string> finishes = new() { AppResources.Gloss, AppResources.Satin, AppResources.SemiMatte, AppResources.Matte };

    [ObservableProperty]
    private ObservableCollection<string> meltFluidities = new() { AppResources.Low, AppResources.Medium, AppResources.High };

    [ObservableProperty]
    private ObservableCollection<string> thicknessTolerances = new() { AppResources.Low, AppResources.Medium, AppResources.High };

    [ObservableProperty]
    private ObservableCollection<string> colorStabilities = new() { AppResources.Stable, AppResources.Variable };

    [ObservableProperty]
    private ObservableCollection<string> repeatabilities = new() { AppResources.Low, AppResources.Medium, AppResources.High };

    [ObservableProperty]
    private ObservableCollection<string> forms = new() { AppResources.DryMix, AppResources.Liquid, AppResources.Brushing };

    [ObservableProperty]
    private ObservableCollection<string> clayInteractions = new() { AppResources.Neutral, AppResources.Contrasting, AppResources.HighlyReactive };

    [ObservableProperty]
    private ObservableCollection<string> workTypes = new() { AppResources.Artistic, AppResources.Functional, AppResources.Both };

    [ObservableProperty]
    private ObservableCollection<string> durabilities = new() { AppResources.Low, AppResources.Medium, AppResources.High };

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

    partial void OnGlazeIdChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            IsEditMode = true;
            Task.Run(async () => await LoadGlazeForEditAsync());
        }
        else
        {
            IsEditMode = false;
        }
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

    private async Task LoadGlazeForEditAsync()
    {
        if (string.IsNullOrEmpty(GlazeId))
            return;

        try
        {
            Glaze = await _glazeService.GetGlazeByIdAsync(GlazeId);
            if (Glaze != null)
            {
                PopulateFieldsFromGlaze();
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleErrorAsync(ex, nameof(LoadGlazeForEditAsync));
        }
    }

    private void PopulateFieldsFromGlaze()
    {
        if (Glaze == null) return;

        // Basic tab
        Name = Glaze.Name;
        Manufacturer = Glaze.Manufacturer;
        BatchDate = Glaze.BatchDate;
        SelectedType = GlazeTypes.FirstOrDefault(t => t.Id == Glaze.TypeId);
        Color = Glaze.Color;
        ConeRating = Glaze.ConeRating;
        Quantity = Glaze.Quantity;
        FoodSafe = Glaze.FoodSafe == true ? AppResources.Yes : Glaze.FoodSafe == false ? AppResources.No : AppResources.NotSpecified;
        IsFavorite = Glaze.IsFavorite;
        Notes = Glaze.Notes;

        // Properties
        if (Glaze.Properties != null)
        {
            // Firing tab
            TemperatureMin = Glaze.Properties.Firing?.TemperatureMin;
            TemperatureMax = Glaze.Properties.Firing?.TemperatureMax;
            TemperatureUnit = Glaze.Properties.Firing?.TemperatureUnit ?? "C";
            Atmosphere = Glaze.Properties.Firing?.Atmosphere;
            CurveSensitivity = Glaze.Properties.Firing?.CurveSensitivity;

            // Appearance tab
            Transparency = Glaze.Properties.Appearance?.Transparency;
            Finish = Glaze.Properties.Appearance?.Finish;
            SelectedTextures = new ObservableCollection<string>(Glaze.Properties.Appearance?.Texture ?? new List<string>());
            SelectedEffects = new ObservableCollection<string>(Glaze.Properties.Appearance?.SpecialEffects ?? new List<string>());

            // Behavior tab
            MeltFluidity = Glaze.Properties.Behavior?.MeltFluidity;
            ThicknessTolerance = Glaze.Properties.Behavior?.ThicknessTolerance;
            ColorStability = Glaze.Properties.Behavior?.ColorStability;
            Repeatability = Glaze.Properties.Behavior?.Repeatability;

            // Application tab
            Form = Glaze.Properties.Application?.Form;
            SelectedMethods = new ObservableCollection<string>(Glaze.Properties.Application?.Methods ?? new List<string>());
            RecommendedThickness = Glaze.Properties.Application?.RecommendedThickness;
            ApplicationNotes = Glaze.Properties.Application?.ApplicationNotes;

            // Advanced tab
            SelectedClayTypes = new ObservableCollection<string>(Glaze.Properties.ClayCompatibility?.BestSuited ?? new List<string>());
            ClayInteraction = Glaze.Properties.ClayCompatibility?.Interaction;
            KnownDefects = new ObservableCollection<string>(Glaze.Properties.Defects?.KnownIssues ?? new List<string>());
            MitigationNotes = Glaze.Properties.Defects?.MitigationNotes;
            WorkType = Glaze.Properties.Usage?.WorkType;
            Durability = Glaze.Properties.Usage?.Durability;
        }
    }

    [RelayCommand]
    private void SelectTab(string tab)
    {
        CurrentTab = tab;
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
                Id = IsEditMode ? Glaze.Id : Guid.NewGuid().ToString(),
                UserId = userId,
                Name = Name,
                Manufacturer = Manufacturer,
                BatchDate = BatchDate,
                TypeId = SelectedType?.Id,
                Color = Color,
                ConeRating = ConeRating,
                Quantity = Quantity,
                Notes = Notes,
                FoodSafe = FoodSafe == AppResources.Yes ? true : FoodSafe == AppResources.No ? false : null,
                IsFavorite = IsFavorite,
                Properties = BuildProperties()
            };

            if (IsEditMode)
            {
                await _glazeService.UpdateGlazeAsync(glaze);
            }
            else
            {
                await _glazeService.CreateGlazeAsync(glaze);
            }

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
