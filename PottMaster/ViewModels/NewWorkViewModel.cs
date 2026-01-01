using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PottMaster.Models;
using PottMaster.Services;
using System.Collections.ObjectModel;

namespace PottMaster.ViewModels;

public partial class NewWorkViewModel : ObservableObject
{
    private readonly IWorkService _workService;
    private readonly IAuthService _authService;
    private readonly IDbService _dbService;

    [ObservableProperty]
    private ObservableCollection<WorkCategory> categories = [];

    [ObservableProperty]
    private WorkCategory? selectedCategory;

    [ObservableProperty]
    private double wallThickness = 5;

    [ObservableProperty]
    private string? photoPath;

    [ObservableProperty]
    private bool isSaving;

    public NewWorkViewModel(IWorkService workService, IAuthService authService, IDbService dbService)
    {
        _workService = workService;
        _authService = authService;
        _dbService = dbService;
    }

    public async Task InitializeAsync()
    {
        await _dbService.InitializeAsync();
        var categoriesList = await _workService.GetCategoriesAsync();
        Categories = new ObservableCollection<WorkCategory>(categoriesList);
    }

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    var localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                    using var stream = await photo.OpenReadAsync();
                    using var newStream = File.OpenWrite(localFilePath);
                    await stream.CopyToAsync(newStream);
                    PhotoPath = localFilePath;
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to take photo: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
                var localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using var stream = await photo.OpenReadAsync();
                using var newStream = File.OpenWrite(localFilePath);
                await stream.CopyToAsync(newStream);
                PhotoPath = localFilePath;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to pick photo: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SaveWorkAsync()
    {
        if (SelectedCategory == null)
        {
            await Shell.Current.DisplayAlert("Validation", "Please select a category", "OK");
            return;
        }

        if (WallThickness < 3 || WallThickness > 50)
        {
            await Shell.Current.DisplayAlert("Validation", "Wall thickness must be between 3 and 50 mm", "OK");
            return;
        }

        IsSaving = true;
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user?.Id == null)
            {
                await Shell.Current.DisplayAlert("Error", "User not authenticated", "OK");
                return;
            }

            var profile = await _dbService.GetByIdAsync<UserProfiles>(user.Id);
            if (profile == null)
            {
                await Shell.Current.DisplayAlert("Error", "User profile not found", "OK");
                return;
            }

            var userInitials = profile.Initials;

            var work = new Work
            {
                UserId = user.Id,
                CategoryId = SelectedCategory.Id,
                WallThickness = (int)WallThickness,
                PhotoPath = PhotoPath
            };

            var code = await _workService.CreateWorkAsync(work, userInitials);

            await Shell.Current.DisplayAlert("Success", $"Work created with code: {code}", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to create work: {ex.Message}", "OK");
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
}
