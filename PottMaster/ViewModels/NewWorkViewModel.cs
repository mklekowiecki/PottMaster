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
    private ObservableCollection<WorkCategory> categories = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    private WorkCategory? selectedCategory;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    private double wallThickness = 5;

    [ObservableProperty]
    private string? photoPath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
    private bool isSaving;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public bool IsSaveEnabled => !isSaving && !isLoading && selectedCategory != null && wallThickness >= 3 && wallThickness <= 50;

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
            Categories = new ObservableCollection<WorkCategory>(categoriesList);
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
            PhotoPath = await _imageService.CompressAndSaveImageAsync(stream, photo.FileName);
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

            var userInitials = profile.Initials;

            var work = new Work
            {
                UserId = user.Id,
                CategoryId = SelectedCategory.Id,
                WallThickness = (int)WallThickness,
                PhotoPath = PhotoPath,
                StatusId = (int)WorkStatusCode.Wet,
                DryingStartedAt = DateTime.UtcNow
            };

            var code = await _workService.CreateWorkAsync(work, userInitials);

            await _alertService.ShowAlertAsync(AppResources.Success, string.Format(AppResources.WorkCreated, code));
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
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    partial void OnSelectedCategoryChanged(WorkCategory? value)
    {
        SaveWorkCommand.NotifyCanExecuteChanged();
    }

    partial void OnWallThicknessChanged(double value)
    {
        SaveWorkCommand.NotifyCanExecuteChanged();
    }
}
