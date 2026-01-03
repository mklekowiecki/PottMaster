using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Services;
using PottMaster.Resources;
using System.Collections.ObjectModel;
using PottMasterLib.Services;
using PottMasterLib.Models;

namespace PottMaster.ViewModels;

public partial class NewWorkViewModel : ObservableObject
{
    private readonly IWorkService _workService;
    private readonly IAuthService _authService;
    private readonly IDbService _dbService;
    private readonly IImageService _imageService;
    private readonly IAlertService _alertService;

    [ObservableProperty]
    private ObservableCollection<LocalWorkCategory> categories = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    private LocalWorkCategory? selectedCategory;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    private double wallThickness = 5;

    [ObservableProperty]
    private ObservableCollection<LocalPhoto> photos = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    private bool isSaving;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    private string? workCode;

    private List<Work> existingWorks = [];
    private string userInitials = "";

    public bool IsSaveEnabled => !isSaving && !isLoading && selectedCategory != null && wallThickness >= 3 && wallThickness <= 50 && !string.IsNullOrWhiteSpace(WorkCode);

    public NewWorkViewModel(IWorkService workService, IAuthService authService, IDbService dbService, IImageService imageService, IAlertService alertService)
    {
        _workService = workService;
        _authService = authService;
        _dbService = dbService;
        _imageService = imageService;
        _alertService = alertService;
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            await _dbService.InitializeAsync();
            var categoriesList = await _workService.GetCategoriesAsync();
            Categories = new ObservableCollection<LocalWorkCategory>(categoriesList);

            var user = await _authService.GetCurrentUserAsync();
            if (user?.Id != null)
            {
                var profile = await _dbService.GetUserProfileByIdAsync(user.Id);
                userInitials = profile?.Initials ?? "";
                existingWorks = await _workService.GetUserWorksAsync(user.Id);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = string.Format(AppResources.Error, ex.Message);
            System.Diagnostics.Debug.WriteLine($"Failed to initialize NewWorkViewModel: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        ErrorMessage = null;

        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    await SavePhotoAsync(photo);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"{AppResources.Error}: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Failed to take photo: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        ErrorMessage = null;

        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
                await SavePhotoAsync(photo);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"{AppResources.Error}: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Failed to pick photo: {ex.Message}");
        }
    }

    private async Task SavePhotoAsync(FileResult photo)
    {
        try
        {
            using var stream = await photo.OpenReadAsync();
            var path = await _imageService.CompressAndSaveImageAsync(stream, photo.FileName);
            var newPhoto = new LocalPhoto
            {
                Path = path,
                Order = Photos.Count
            };
            Photos.Add(newPhoto);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to compress and save photo: {ex.Message}");
            throw;
        }
    }

    [RelayCommand(CanExecute = nameof(IsSaveEnabled))]
    private async Task SaveWorkAsync()
    {
        if (SelectedCategory == null)
        {
            await _alertService.ShowAlertAsync(AppResources.Validation, AppResources.SelectCategoryValidation);
            return;
        }

        if (WallThickness < 3 || WallThickness > 50)
        {
            await _alertService.ShowAlertAsync(AppResources.Validation, AppResources.WallThicknessValidation);
            return;
        }

        IsSaving = true;
        ErrorMessage = null;

        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user?.Id == null)
            {
                await _alertService.ShowAlertAsync(AppResources.Error, AppResources.UserNotLoggedIn);
                return;
            }

            var profile = await _dbService.GetUserProfileByIdAsync(user.Id);
            if (profile == null)
            {
                await _alertService.ShowAlertAsync(AppResources.Error, AppResources.UserProfileNotFound);
                return;
            }

            userInitials = profile.Initials;

            var work = new Work
            {
                UserId = user.Id,
                CategoryId = SelectedCategory.Id,
                WallThickness = (int)WallThickness,
                StatusId = (int)WorkStatusCode.Wet,
                DryingStartedAt = DateTime.UtcNow,
                Code = WorkCode,
                PhotoPath = Photos.FirstOrDefault()?.Path ?? ""
            };

            var createdWork = await _workService.CreateWorkAsync(work, userInitials);

            // Save photos
            foreach (var photo in Photos)
            {
                photo.WorkId = createdWork.Id;
                await _dbService.InsertPhotoAsync(photo);
            }

            await _alertService.ShowAlertAsync(AppResources.Success, string.Format(AppResources.WorkCreated, createdWork.Code));
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = string.Format(AppResources.Error, ex.Message);
            await _alertService.ShowAlertAsync(AppResources.Error, string.Format(AppResources.WorkCreationFailed, ex.Message));
            System.Diagnostics.Debug.WriteLine($"Failed to create work: {ex.Message}");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task RemovePhotoAsync(LocalPhoto photo)
    {
        Photos.Remove(photo);
        // Optionally delete the file
        if (!string.IsNullOrEmpty(photo.Path) && File.Exists(photo.Path))
        {
            File.Delete(photo.Path);
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    partial void OnSelectedCategoryChanged(LocalWorkCategory? value)
    {
        if (value != null && !string.IsNullOrEmpty(userInitials))
        {
            WorkCode = GenerateWorkCode(userInitials, value.Code, existingWorks);
        }
        SaveWorkCommand.NotifyCanExecuteChanged();
    }

    partial void OnWallThicknessChanged(double value)
    {
        SaveWorkCommand.NotifyCanExecuteChanged();
    }

    private string GenerateWorkCode(string userInitials, string categoryCode, IEnumerable<Work> existingWorks)
    {
        var monthYear = DateTime.UtcNow.ToString("MMyy");

        var filteredWorks = existingWorks
            .Where(w => w.Code.StartsWith($"{userInitials}-{categoryCode}-{monthYear}-"))
            .ToList();

        var counter = filteredWorks.Count + 1;
        return $"{userInitials}-{categoryCode}-{monthYear}-{counter:D3}";
    }
}
