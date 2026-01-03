using PottMasterLib.Models;

namespace PottMaster.Services;

public interface IWorkService
{
    Task<List<Work>> GetUserWorksAsync(string userId);
    Task<Work?> GetWorkByIdAsync(string workId);
    Task<Work> CreateWorkAsync(Work work, string userInitials);
    Task UpdateWorkAsync(Work work);
    Task DeleteWorkAsync(string workId);
    Task<List<LocalWorkCategory>> GetCategoriesAsync();
    Task<List<LocalWorkStatus>> GetStatusesAsync();
}
