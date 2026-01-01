using PottMaster.Models;

namespace PottMaster.Services;

public interface IWorkService
{
    Task<List<Work>> GetUserWorksAsync(string userId);
    Task<Work?> GetWorkByIdAsync(string workId);
    Task<string> CreateWorkAsync(Work work, string userInitials);
    Task UpdateWorkAsync(Work work);
    Task DeleteWorkAsync(string workId);
    Task<List<WorkCategory>> GetCategoriesAsync();
    Task<List<WorkStatus>> GetStatusesAsync();
}
