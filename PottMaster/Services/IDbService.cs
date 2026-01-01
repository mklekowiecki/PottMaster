using SQLite;
using PottMaster.Models;

namespace PottMaster.Services;

public interface IDbService
{
    Task InitializeAsync();
    Task<List<T>> GetAllAsync<T>() where T : new();
    Task<T?> GetByIdAsync<T>(string id) where T : new();
    Task<int> InsertAsync<T>(T entity);
    Task<int> UpdateAsync<T>(T entity);
    Task<int> DeleteAsync<T>(T entity);
    Task<List<LocalWork>> GetWorksByUserIdAsync(string userId);
    Task<WorkCategory?> GetWorkCategoryByIdAsync(int categoryId);
    Task<WorkStatus?> GetWorkStatusByIdAsync(int statusId);
}
