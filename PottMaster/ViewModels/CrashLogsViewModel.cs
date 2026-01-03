using PottMaster.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PottMaster.ViewModels;

public partial class CrashLogsViewModel : ObservableObject
{
    private readonly GlobalExceptionHandler _exceptionHandler;
    
    [ObservableProperty]
    private ObservableCollection<CrashLogFile> logFiles = new();
    
    [ObservableProperty]
    private bool hasLogs;
    
    public CrashLogsViewModel(GlobalExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;
    }

    [RelayCommand]
    public async Task LoadLogFilesAsync()
    {
        LogFiles.Clear();
        
        var files = GlobalExceptionHandler.GetLogFiles();
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            LogFiles.Add(new CrashLogFile
            {
                FileName = fileInfo.Name,
                FilePath = file,
                CreatedAt = fileInfo.CreationTime,
                Size = fileInfo.Length
            });
        }
        
        HasLogs = LogFiles.Count > 0;
    }

    [RelayCommand]
    public async Task ViewLogAsync(string filePath)
    {
        var content = GlobalExceptionHandler.ReadLogFile(filePath);
        if (!string.IsNullOrEmpty(content))
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Crash Log",
                content.Length > 1000 ? content.Substring(0, 1000) + "...\n\n(Log truncated, use Share to see full content)" : content,
                "OK");
        }
    }

    [RelayCommand]
    public async Task ShareLogAsync(string filePath)
    {
        if (File.Exists(filePath))
        {
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share Crash Log",
                File = new ShareFile(filePath)
            });
        }
    }

    [RelayCommand]
    public async Task DeleteLogAsync(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                await LoadLogFilesAsync();
            }
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Failed to delete log: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public async Task DeleteAllLogsAsync()
    {
        var confirmed = await Application.Current!.MainPage!.DisplayAlert(
            "Confirm Delete", 
            "Are you sure you want to delete all crash logs?", 
            "Yes", 
            "No");
            
        if (confirmed)
        {
            try
            {
                var files = GlobalExceptionHandler.GetLogFiles().ToList();
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                await LoadLogFilesAsync();
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Failed to delete logs: {ex.Message}", "OK");
            }
        }
    }

    public string GetLogDirectory() => GlobalExceptionHandler.GetLogDirectoryPath();
}

public class CrashLogFile
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long Size { get; set; }
    
    public string DisplaySize => Size < 1024 ? $"{Size} B" : 
                                 Size < 1024 * 1024 ? $"{Size / 1024} KB" : 
                                 $"{Size / (1024 * 1024)} MB";
    
    public string DisplayDate => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
}
