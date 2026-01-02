using PottMasterLib.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Nelibur.ObjectMapper;

namespace PottMasterLib.Services;

public class SyncService : ISyncService
{
    private readonly IDbService _dbService;
    private readonly Supabase.Client _supabaseClient;
    private readonly ILogger<SyncService> _logger;
    private bool _isSyncing = false;

    public SyncService(IDbService dbService, Supabase.Client supabaseClient, ILogger<SyncService> logger)
    {
        _dbService = dbService;
        _supabaseClient = supabaseClient;
        _logger = logger;
        TinyMapper.Bind<LocalWork, RemoteWork>();
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
            // Sync dictionaries first
            await SyncDictionariesAsync();

            // Sync pending works
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

            var workToSync = TinyMapper.Map<RemoteWork>(work);

            await _supabaseClient
                .From<RemoteWork>()
                .Upsert(workToSync);

            work.SyncStatus = SyncStatus.Synced.Code();
            await _dbService.UpdateAsync(work);

            Debug.WriteLine($"Successfully synced work: {work.Code}");
            return true;
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
            _logger.LogError(ex, "Failed to sync user profile");
            return false;
        }
    }
}
