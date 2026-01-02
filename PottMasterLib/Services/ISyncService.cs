using PottMasterLib.Models;

namespace PottMasterLib.Services;

public interface ISyncService
{
    Task<bool> IsOnlineAsync();
    Task SyncPendingChangesAsync();
    Task<bool> SyncWorkAsync(LocalWork work);
    Task<bool> SyncUserProfileAsync(LocalUserProfile profile);
    Task<int> GetPendingSyncCountAsync();
}
