using Microsoft.Extensions.Logging;
using PottMasterLib.Services;
using PottMasterLib.Models;

namespace PottMaster.Repositories;

public class LocalWorkRepository : IWorkRepository
{
    private readonly IDbService _dbService;
    private readonly ILogger<LocalWorkRepository> _logger;
    
    public LocalWorkRepository(IDbService dbService, ILogger<LocalWorkRepository> logger)
    {
        _dbService = dbService;
        _logger = logger;
    }
    
    public async Task<Result<List<Work>>> GetByUserIdAsync(string userId)
    {
        try
        {
            var localWorks = await _dbService.GetWorksByUserIdAsync(userId);
            var works = new List<Work>();
            
            foreach (var localWork in localWorks)
            {
                var category = await _dbService.GetWorkCategoryByIdAsync(localWork.CategoryId);
                localWork.CategoryCode = category?.Code ?? "";
                
                var status = await _dbService.GetWorkStatusByIdAsync(localWork.StatusId);
                localWork.StatusCode = status?.Code ?? "";
                
                works.Add(MapToWork(localWork));
            }
            
            return Result<List<Work>>.Success(works);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get works for user {UserId}", userId);
            return Result<List<Work>>.Failure("Failed to load works", ex);
        }
    }
    
    public async Task<Result<Work>> GetByIdAsync(string workId)
    {
        try
        {
            var localWork = await _dbService.GetByIdAsync<LocalWork>(workId);
            
            if (localWork == null)
            {
                return Result<Work>.Failure("Work not found");
            }
            
            var category = await _dbService.GetWorkCategoryByIdAsync(localWork.CategoryId);
            localWork.CategoryCode = category?.Code ?? "";
            
            var status = await _dbService.GetWorkStatusByIdAsync(localWork.StatusId);
            localWork.StatusCode = status?.Code ?? "";
            
            return Result<Work>.Success(MapToWork(localWork));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work {WorkId}", workId);
            return Result<Work>.Failure("Failed to load work", ex);
        }
    }
    
    public async Task<Result<string>> CreateAsync(Work work, string userInitials)
    {
        try
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
            localWork.Id = Guid.NewGuid().ToString();
            localWork.StatusId = 1;
            localWork.CreatedAt = DateTime.UtcNow;
            localWork.UpdatedAt = DateTime.UtcNow;
            localWork.SyncStatus = "PENDING";
            
            await _dbService.InsertAsync(localWork);
            
            _logger.LogInformation("Work created with code: {WorkCode}", work.Code);
            return Result<string>.Success(work.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create work");
            return Result<string>.Failure("Failed to create work", ex);
        }
    }
    
    public async Task<Result> UpdateAsync(Work work)
    {
        try
        {
            var localWork = MapToLocalWork(work);
            localWork.UpdatedAt = DateTime.UtcNow;
            localWork.SyncStatus = "PENDING";
            
            await _dbService.UpdateAsync(localWork);
            
            _logger.LogInformation("Work updated: {WorkId}", work.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update work {WorkId}", work.Id);
            return Result.Failure("Failed to update work", ex);
        }
    }
    
    public async Task<Result> DeleteAsync(string workId)
    {
        try
        {
            var localWork = await _dbService.GetByIdAsync<LocalWork>(workId);
            if (localWork == null)
            {
                return Result.Failure("Work not found");
            }
            
            await _dbService.DeleteAsync(localWork);
            
            _logger.LogInformation("Work deleted: {WorkId}", workId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete work {WorkId}", workId);
            return Result.Failure("Failed to delete work", ex);
        }
    }
    
    public async Task<Result<List<WorkCategory>>> GetCategoriesAsync()
    {
        try
        {
            var categories = await _dbService.GetAllAsync<WorkCategory>();
            return Result<List<WorkCategory>>.Success(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get categories");
            return Result<List<WorkCategory>>.Failure("Failed to load categories", ex);
        }
    }
    
    public async Task<Result<List<WorkStatus>>> GetStatusesAsync()
    {
        try
        {
            var statuses = await _dbService.GetAllAsync<WorkStatus>();
            return Result<List<WorkStatus>>.Success(statuses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get statuses");
            return Result<List<WorkStatus>>.Failure("Failed to load statuses", ex);
        }
    }
    
    private Work MapToWork(LocalWork localWork)
    {
        return new Work
        {
            Id = localWork.Id,
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
            CategoryCode = localWork.CategoryCode,
            StatusCode = localWork.StatusCode
        };
    }
    
    private LocalWork MapToLocalWork(Work work)
    {
        return new LocalWork
        {
            Id = work.Id,
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
