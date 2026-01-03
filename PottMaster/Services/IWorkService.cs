using PottMasterLib.Models;

namespace PottMaster.Services;

public interface IWorkService
{
    Task<List<LocalWork>> GetUserWorksAsync(string userId);
    Task<LocalWork?> GetWorkByIdAsync(string workId);
    Task<LocalWork> CreateWorkAsync(LocalWork work, string userInitials);
    Task UpdateWorkAsync(LocalWork work);
    Task DeleteWorkAsync(string workId);
    Task<List<LocalWorkCategory>> GetCategoriesAsync();
    Task<List<LocalWorkStatus>> GetStatusesAsync();
}
