using PottMaster.Models;

namespace PottMaster.Services;

public interface ISyncService
{
    Task<bool> IsOnlineAsync();
    Task SyncPendingChangesAsync();
    Task<bool> SyncWorkAsync(LocalWork work);
    Task<bool> SyncUserProfileAsync(LocalUserProfile profile);
    int GetPendingSyncCount();
}
