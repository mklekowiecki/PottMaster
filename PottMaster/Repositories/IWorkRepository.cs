using PottMasterLib.Models;

namespace PottMaster.Repositories;

public interface IWorkRepository
{
    Task<Result<List<LocalWork>>> GetByUserIdAsync(string userId);
    Task<Result<LocalWork>> GetByIdAsync(string workId);
    Task<Result<LocalWork>> CreateAsync(LocalWork work, string userInitials);
    Task<Result> UpdateAsync(LocalWork work);
    Task<Result> DeleteAsync(string workId);
    Task<Result<List<LocalWorkCategory>>> GetCategoriesAsync();
    Task<Result<List<LocalWorkStatus>>> GetStatusesAsync();
}
