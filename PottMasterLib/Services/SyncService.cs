using PottMasterLib.Models;
using PottMasterLib.Services;
using System.Collections.Generic;
using System.Diagnostics;

namespace PottMasterLib.Services;

public class SyncService : ISyncService
{
    private readonly IDbService _dbService;
    private readonly Supabase.Client _supabaseClient;
    private bool _isSyncing = false;

    public SyncService(IDbService dbService, Supabase.Client supabaseClient)
    {
        _dbService = dbService;
        _supabaseClient = supabaseClient;
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
                .Count(w => w.SyncStatus == "PENDING" || w.SyncStatus == "ERROR");
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
            // Fetch work categories from Supabase
            var categoriesResponse = await _supabaseClient
                .From<WorkCategory>()
                .Get();
            var categories = categoriesResponse.Models;
            if (categories != null)
            {
                await _dbService.UpsertAllAsync(categories);
                Debug.WriteLine($"Synced {categories.Count} work categories.");
            }

            // Fetch work statuses from Supabase
            var statusesResponse = await _supabaseClient
                .From<WorkStatus>()
                .Get();
            var statuses = statusesResponse.Models;
            if (statuses != null)
            {
                await _dbService.UpsertAllAsync(statuses);
                Debug.WriteLine($"Synced {statuses.Count} work statuses.");
            }

            // Fetch wiki material types from Supabase
            var materialTypesResponse = await _supabaseClient
                .From<WikiMaterialType>()
                .Get();
            var materialTypes = materialTypesResponse.Models;
            if (materialTypes != null)
            {
                await _dbService.UpsertAllAsync(materialTypes);
                Debug.WriteLine($"Synced {materialTypes.Count} wiki material types.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to sync dictionaries: {ex.Message}");
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
            // Sync dictionaries first
            await SyncDictionariesAsync();

            // Sync pending works
            var pendingWorks = (await _dbService.GetAllAsync<LocalWork>())
                .Where(w => w.SyncStatus == "PENDING" || w.SyncStatus == "ERROR")
                .ToList();

            foreach (var work in pendingWorks)
            {
                await SyncWorkAsync(work);
            }

            Debug.WriteLine($"Sync completed. Synced {pendingWorks.Count} works.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Sync error: {ex.Message}");
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
            work.SyncStatus = "SYNCING";
            await _dbService.UpdateAsync(work);

            var workToSync = MapLocalWorkToRemoteWork(work);

            await _supabaseClient
                .From<RemoteWork>()
                .Upsert(workToSync);

            work.SyncStatus = "SYNCED";
            await _dbService.UpdateAsync(work);

            Debug.WriteLine($"Successfully synced work: {work.Code}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to sync work {work.Code}: {ex.Message}");
            
            work.SyncStatus = "ERROR";
            await _dbService.UpdateAsync(work);
            
            return false;
        }
    }

    public async Task<bool> SyncUserProfileAsync(LocalUserProfile profile)
    {
        try
        {
            var remoteProfile = new UserProfiles
            {
                Id = profile.Id,
                Email = profile.Email,
                Initials = profile.Initials,
                CreatedAt = profile.CreatedAt
            };

            await _supabaseClient
                .From<UserProfiles>()
                .Upsert(remoteProfile);

            Debug.WriteLine($"Successfully synced user profile: {profile.Email}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to sync user profile: {ex.Message}");
            return false;
        }
    }

    private RemoteWork MapLocalWorkToRemoteWork(LocalWork localWork)
    {
        return new RemoteWork
        {
            Id = localWork.Id,
            UserId = localWork.UserId,
            Code = localWork.Code,
            CategoryId = localWork.CategoryId,
            WallThickness = localWork.WallThickness,
            PhotoPath = localWork.PhotoPath,
            StatusId = localWork.StatusId,
            CreatedAt = localWork.CreatedAt,
            DryingStartedAt = localWork.DryingStartedAt,
            DryingCompletedAt = localWork.DryingCompletedAt,
            SyncStatus = "SYNCED",
            UpdatedAt = localWork.UpdatedAt
        };
    }
}
