using PottMaster.Models;

namespace PottMaster.Services;

public class WorkService : IWorkService
{
    private readonly IDbService _dbService;

    public WorkService(IDbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<List<Work>> GetUserWorksAsync(string userId)
    {
        var works = await _dbService.QueryAsync<Work>(
            "SELECT * FROM works WHERE UserId = ? ORDER BY CreatedAt DESC", userId);

        foreach (var work in works)
        {
            var category = await _dbService.QueryAsync<WorkCategory>(
                "SELECT * FROM work_categories WHERE Id = ?", work.CategoryId);
            work.CategoryName = category.FirstOrDefault()?.Name ?? "Unknown";

            var status = await _dbService.QueryAsync<WorkStatus>(
                "SELECT * FROM work_statuses WHERE Id = ?", work.StatusId);
            work.StatusName = status.FirstOrDefault()?.Name ?? "Unknown";
        }

        return works;
    }

    public async Task<Work?> GetWorkByIdAsync(string workId)
    {
        var work = await _dbService.GetByIdAsync<Work>(workId);
        
        if (work != null)
        {
            var category = await _dbService.QueryAsync<WorkCategory>(
                "SELECT * FROM work_categories WHERE Id = ?", work.CategoryId);
            work.CategoryName = category.FirstOrDefault()?.Name ?? "Unknown";

            var status = await _dbService.QueryAsync<WorkStatus>(
                "SELECT * FROM work_statuses WHERE Id = ?", work.StatusId);
            work.StatusName = status.FirstOrDefault()?.Name ?? "Unknown";
        }

        return work;
    }

    public async Task<string> CreateWorkAsync(Work work, string userInitials)
    {
        var category = await _dbService.QueryAsync<WorkCategory>(
            "SELECT * FROM work_categories WHERE Id = ?", work.CategoryId);
        var categoryCode = category.FirstOrDefault()?.Code ?? "OTH";

        var monthYear = DateTime.UtcNow.ToString("MMyy");
        
        var existingWorks = await _dbService.QueryAsync<Work>(
            "SELECT * FROM works WHERE UserId = ? AND Code LIKE ?",
            work.UserId, $"{userInitials}-{categoryCode}-{monthYear}-%");

        var counter = existingWorks.Count + 1;
        work.Code = $"{userInitials}-{categoryCode}-{monthYear}-{counter:D3}";
        
        work.Id = Guid.NewGuid().ToString();
        work.StatusId = 1;
        work.CreatedAt = DateTime.UtcNow;
        work.UpdatedAt = DateTime.UtcNow;
        work.SyncStatus = "PENDING";

        await _dbService.InsertAsync(work);

        return work.Code;
    }

    public async Task UpdateWorkAsync(Work work)
    {
        work.UpdatedAt = DateTime.UtcNow;
        work.SyncStatus = "PENDING";
        await _dbService.UpdateAsync(work);
    }

    public async Task DeleteWorkAsync(string workId)
    {
        var work = await _dbService.GetByIdAsync<Work>(workId);
        if (work != null)
        {
            await _dbService.DeleteAsync(work);
        }
    }

    public async Task<List<WorkCategory>> GetCategoriesAsync()
    {
        return await _dbService.GetAllAsync<WorkCategory>();
    }

    public async Task<List<WorkStatus>> GetStatusesAsync()
    {
        return await _dbService.GetAllAsync<WorkStatus>();
    }
}
