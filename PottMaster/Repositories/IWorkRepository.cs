using PottMasterLib.Models;

namespace PottMaster.Repositories;

public interface IWorkRepository
{
    Task<Result<List<Work>>> GetByUserIdAsync(string userId);
    Task<Result<Work>> GetByIdAsync(string workId);
    Task<Result<Work>> CreateAsync(Work work, string userInitials);
    Task<Result> UpdateAsync(Work work);
    Task<Result> DeleteAsync(string workId);
    Task<Result<List<LocalWorkCategory>>> GetCategoriesAsync();
    Task<Result<List<LocalWorkStatus>>> GetStatusesAsync();
}
