using PottMaster.Models;

namespace PottMaster.Repositories;

public interface IWorkRepository
{
    Task<Result<List<Work>>> GetByUserIdAsync(string userId);
    Task<Result<Work>> GetByIdAsync(string workId);
    Task<Result<string>> CreateAsync(Work work, string userInitials);
    Task<Result> UpdateAsync(Work work);
    Task<Result> DeleteAsync(string workId);
    Task<Result<List<WorkCategory>>> GetCategoriesAsync();
    Task<Result<List<WorkStatus>>> GetStatusesAsync();
}
