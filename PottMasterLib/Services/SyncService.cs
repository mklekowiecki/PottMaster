using PottMasterLib.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;


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
}
