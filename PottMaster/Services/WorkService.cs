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
        var localWorks = await _dbService.GetWorksByUserIdAsync(userId);
        var works = new List<Work>();

        foreach (var localWork in localWorks)
        {
            var category = await _dbService.GetWorkCategoryByIdAsync(localWork.CategoryId);
            localWork.CategoryName = category?.Name ?? "Unknown";

            var status = await _dbService.GetWorkStatusByIdAsync(localWork.StatusId);
            localWork.StatusName = status?.Name ?? "Unknown";

            works.Add(MapToWork(localWork));
        }

        return works;
    }

    public async Task<Work?> GetWorkByIdAsync(string workId)
    {
        var localWork = await _dbService.GetByIdAsync<LocalWork>(workId);

        if (localWork != null)
        {
            var category = await _dbService.GetWorkCategoryByIdAsync(localWork.CategoryId);
            localWork.CategoryName = category?.Name ?? "Unknown";

            var status = await _dbService.GetWorkStatusByIdAsync(localWork.StatusId);
            localWork.StatusName = status?.Name ?? "Unknown";

            return MapToWork(localWork);
        }

        return null;
    }

    public async Task<string> CreateWorkAsync(Work work, string userInitials)
    {
        var category = await _dbService.GetWorkCategoryByIdAsync(work.CategoryId);
        var categoryCode = category?.Code ?? "OTH";

        var monthYear = DateTime.UtcNow.ToString("MMyy");

        var existingWorks = await _dbService.GetWorksByUserIdAsync(work.UserId);
        var filteredWorks = existingWorks
            .Where(w => w.Code.StartsWith($"{userInitials}-{categoryCode}-{monthYear}-"))
            .ToList();

        var counter = filteredWorks.Count + 1;
        work.Code = $"{userInitials}-{categoryCode}-{monthYear}-{counter:D3}";

        var localWork = MapToLocalWork(work);
        localWork.Id = 0; // SQLite auto-increment
        localWork.StatusId = 1;
        localWork.CreatedAt = DateTime.UtcNow;
        localWork.UpdatedAt = DateTime.UtcNow;
        localWork.SyncStatus = "PENDING";

        await _dbService.InsertAsync(localWork);

        return work.Code;
    }

    public async Task UpdateWorkAsync(Work work)
    {
        var localWork = MapToLocalWork(work);
        localWork.UpdatedAt = DateTime.UtcNow;
        localWork.SyncStatus = "PENDING";
        await _dbService.UpdateAsync(localWork);
    }

    public async Task DeleteWorkAsync(string workId)
    {
        var localWork = await _dbService.GetByIdAsync<LocalWork>(workId);
        if (localWork != null)
        {
            await _dbService.DeleteAsync(localWork);
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

    private Work MapToWork(LocalWork localWork)
    {
        return new Work
        {
            Id = localWork.Id.ToString(),
            UserId = localWork.UserId,
            Code = localWork.Code,
            CategoryId = localWork.CategoryId,
            WallThickness = localWork.WallThickness,
            PhotoPath = localWork.PhotoPath,
            StatusId = localWork.StatusId,
            CreatedAt = localWork.CreatedAt,
            DryingStartedAt = localWork.DryingStartedAt,
            DryingCompletedAt = localWork.DryingCompletedAt,
            SyncStatus = localWork.SyncStatus,
            UpdatedAt = localWork.UpdatedAt,
            CategoryName = localWork.CategoryName,
            StatusName = localWork.StatusName
        };
    }

    private LocalWork MapToLocalWork(Work work)
    {
        return new LocalWork
        {
            Id = int.TryParse(work.Id, out var id) ? id : 0,
            UserId = work.UserId,
            Code = work.Code,
            CategoryId = work.CategoryId,
            WallThickness = work.WallThickness,
            PhotoPath = work.PhotoPath,
            StatusId = work.StatusId,
            CreatedAt = work.CreatedAt,
            DryingStartedAt = work.DryingStartedAt,
            DryingCompletedAt = work.DryingCompletedAt,
            SyncStatus = work.SyncStatus,
            UpdatedAt = work.UpdatedAt
        };
    }
}
