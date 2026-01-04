using PottMasterLib.Logic;
using PottMasterLib.Models;
using PottMasterLib.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using System.IO;


namespace PottMasterLib.Services;

public class SyncService : ISyncService
{
    private readonly IDbService _dbService;
    private readonly IApiEndpoint _apiEndpoint;
    private readonly ILogger<SyncService> _logger;
    private bool _isSyncing = false;

    public SyncService(IDbService dbService, IApiEndpoint apiEndpoint, ILogger<SyncService> logger)
    {
        _dbService = dbService;
        _apiEndpoint = apiEndpoint;
        _logger = logger;
    }

    public async Task<bool> IsOnlineAsync()
    {
        try
        {
            var current = Connectivity.Current.NetworkAccess;
            return current == NetworkAccess.Internet;
        }
        catch
        {
            return false;
        }
    }

    public async Task<int> GetPendingSyncCountAsync()
    {
        try
        {
            return (await _dbService.GetAllAsync<LocalWork>())
                .Count(w => w.SyncStatus == SyncStatus.Pending.Code() || w.SyncStatus == SyncStatus.Error.Code());
        }
        catch
        {
            return 0;
        }
    }

    public async Task SyncDictionariesAsync()
    {
        try
        {
            var localCategories = await _dbService.GetAllAsync<LocalWorkCategory>();
            if (!localCategories.Any())
            {
                var categoriesResult = await _apiEndpoint.GetWorkCategoriesAsync();
                if (categoriesResult.IsSuccess && categoriesResult.Value != null)
                {
                    var localCategoriesToUpsert = categoriesResult.Value.Select(c => new LocalWorkCategory
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Code = c.Code
                    }).ToList();
                    await _dbService.UpsertAllAsync(localCategoriesToUpsert);
                    Debug.WriteLine($"Synced {categoriesResult.Value.Count} work categories.");
                }
            }

            var localStatuses = await _dbService.GetAllAsync<LocalWorkStatus>();
            if (!localStatuses.Any())
            {
                var statusesResult = await _apiEndpoint.GetWorkStatusesAsync();
                if (statusesResult.IsSuccess && statusesResult.Value != null)
                {
                    var localStatusesToUpsert = statusesResult.Value.Select(s => new LocalWorkStatus
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code = s.Code
                    }).ToList();
                    await _dbService.UpsertAllAsync(localStatusesToUpsert);
                    Debug.WriteLine($"Synced {statusesResult.Value.Count} work statuses.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync dictionaries");
        }
    }

    public async Task SyncPendingChangesAsync()
    {
        if (_isSyncing)
        {
            Debug.WriteLine("Sync already in progress, skipping...");
            return;
        }

        if (!await IsOnlineAsync())
        {
            Debug.WriteLine("No internet connection, skipping sync");
            return;
        }

        _isSyncing = true;

        try
        {
            await SyncDictionariesAsync();

            var pendingWorks = (await _dbService.GetAllAsync<LocalWork>())
                .Where(w => w.SyncStatus == SyncStatus.Pending.Code() || w.SyncStatus == SyncStatus.Error.Code())
                .ToList();

            foreach (var work in pendingWorks)
            {
                await SyncWorkAsync(work);
            }

            Debug.WriteLine($"Sync completed. Synced {pendingWorks.Count} works.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync error");
        }
        finally
        {
            _isSyncing = false;
        }
    }

    public async Task<bool> SyncWorkAsync(LocalWork work)
    {
        try
        {
            work.SyncStatus = SyncStatus.Syncing.Code();
            await _dbService.UpdateAsync(work);

            var result = await _apiEndpoint.UpsertWorkAsync(work);

            if (result.IsSuccess)
            {
                // Sync photos after work is synced
                await SyncPhotosForWorkAsync(work.Id);

                work.SyncStatus = SyncStatus.Synced.Code();
                await _dbService.UpdateAsync(work);

                Debug.WriteLine($"Successfully synced work: {work.Code}");
                return true;
            }
            else
            {
                work.SyncStatus = SyncStatus.Error.Code();
                await _dbService.UpdateAsync(work);
                _logger.LogError("Failed to sync work {Code}: {Error}", work.Code ?? "Unknown", result.Error ?? "Unknown error");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync work {Code}", work.Code);

            work.SyncStatus = SyncStatus.Error.Code();
            await _dbService.UpdateAsync(work);

            return false;
        }
    }

    private async Task SyncPhotosForWorkAsync(string workId)
    {
        try
        {
            var photos = await _dbService.GetPhotosByWorkIdAsync(workId);
            var pendingPhotos = photos.Where(p => p.SyncStatus == SyncStatus.Pending.Code() || p.SyncStatus == SyncStatus.Error.Code()).ToList();

            foreach (var photo in pendingPhotos)
            {
                await SyncPhotoAsync(photo);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync photos for work {WorkId}", workId);
        }
    }

    private async Task<bool> SyncPhotoAsync(LocalPhoto photo)
    {
        try
        {
            // Upload to Supabase
            var uploadResult = await _apiEndpoint.UploadPhotoAsync(photo);
            if (uploadResult.IsSuccess)
            {
                photo.RemotePath = uploadResult.Value;
                photo.SyncStatus = SyncStatus.Synced.Code();
                await _dbService.UpdatePhotoAsync(photo);

                Debug.WriteLine($"Successfully synced photo: {photo.Id}");
                return true;
            }
            else
            {
                _logger.LogError("Failed to upload photo {PhotoId}: {Error}", photo.Id, uploadResult.Error);
                photo.SyncStatus = SyncStatus.Error.Code();
                await _dbService.UpdatePhotoAsync(photo);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync photo {PhotoId}", photo.Id);

            photo.SyncStatus = SyncStatus.Error.Code();
            await _dbService.UpdatePhotoAsync(photo);

            return false;
        }
    }

    public async Task<bool> SyncUserProfileAsync(LocalUserProfile profile)
    {
        try
        {
            var result = await _apiEndpoint.UpsertUserProfileAsync(profile);

            if (result.IsSuccess)
            {
                Debug.WriteLine($"Successfully synced user profile: {profile.Email}");
                return true;
            }
            else
            {
                _logger.LogError("Failed to sync user profile: {Error}", result.Error);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync user profile");
            return false;
        }
    }

    public async Task SyncWorksFromServerAsync(string userId)
    {
        if (_isSyncing)
        {
            Debug.WriteLine("Sync already in progress, skipping...");
            return;
        }

        if (!await IsOnlineAsync())
        {
            Debug.WriteLine("No internet connection, skipping sync");
            return;
        }

        _isSyncing = true;

        try
        {
            var worksResult = await _apiEndpoint.GetWorksByUserIdAsync(userId);
            if (!worksResult.IsSuccess || worksResult.Value == null)
            {
                Debug.WriteLine("Failed to get works from server");
                return;
            }

            foreach (var work in worksResult.Value)
            {
                var localWork = await _dbService.GetByIdAsync<LocalWork>(work.Id);
                if (localWork == null || work.UpdatedAt > localWork.UpdatedAt)
                {
                    var localWorkToUpsert = new LocalWork
                    {
                        Id = work.Id,
                        UserId = work.UserId,
                        Code = work.Code,
                        CategoryId = work.CategoryId,
                        WallThickness = work.WallThickness,
                        PhotoPath = null, // Will set after download
                        StatusId = work.StatusId,
                        CreatedAt = work.CreatedAt,
                        DryingStartedAt = work.DryingStartedAt,
                        DryingCompletedAt = work.DryingCompletedAt,
                        SyncStatus = SyncStatus.Synced.Code(),
                        UpdatedAt = work.UpdatedAt
                    };

                    await _dbService.UpsertAllAsync(new[] { localWorkToUpsert });

                    // Download main photo if exists
                    if (!string.IsNullOrEmpty(work.PhotoPath))
                    {
                        var downloadResult = await _apiEndpoint.DownloadPhotoAsync(work.PhotoPath);
                        if (downloadResult.IsSuccess && downloadResult.Value != null)
                        {
                            var localPath = Path.Combine(FileSystem.AppDataDirectory, "images", work.Id + "_main.jpg");
                            Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
                            await File.WriteAllBytesAsync(localPath, downloadResult.Value);
                            localWorkToUpsert.PhotoPath = localPath;
                            await _dbService.UpdateAsync(localWorkToUpsert);
                        }
                    }

                    // Sync photos for this work
                    await SyncPhotosForWorkFromServerAsync(work.Id);
                }
            }

            Debug.WriteLine($"Downloaded {worksResult.Value.Count} works.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync works from server");
        }
        finally
        {
            _isSyncing = false;
        }
    }

    private async Task SyncPhotosForWorkFromServerAsync(string workId)
    {
        try
        {
            var photosResult = await _apiEndpoint.GetPhotosByWorkIdAsync(workId);
            if (!photosResult.IsSuccess || photosResult.Value == null)
            {
                return;
            }

            foreach (var photo in photosResult.Value)
            {
                var localPhoto = await _dbService.GetByIdAsync<LocalPhoto>(photo.Id);
                if (localPhoto == null || photo.UpdatedAt > localPhoto.UpdatedAt)
                {
                    var downloadResult = await _apiEndpoint.DownloadPhotoAsync(photo.RemotePath);
                    if (downloadResult.IsSuccess && downloadResult.Value != null)
                    {
                        var localPath = Path.Combine(FileSystem.AppDataDirectory, "images", photo.Id + ".jpg");
                        Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
                        await File.WriteAllBytesAsync(localPath, downloadResult.Value);

                        var localPhotoToUpsert = new LocalPhoto
                        {
                            Id = photo.Id,
                            WorkId = photo.WorkId,
                            RemotePath = photo.RemotePath,
                            Path = localPath,
                            Order = photo.Order,
                            SyncStatus = SyncStatus.Synced.Code(),
                            UpdatedAt = photo.UpdatedAt
                        };

                        await _dbService.UpsertAllAsync(new[] { localPhotoToUpsert });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync photos for work {WorkId}", workId);
        }
    }
}
