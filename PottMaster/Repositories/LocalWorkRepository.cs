using Microsoft.Extensions.Logging;
using PottMasterLib.Services;
using PottMasterLib.Models;
using PottMasterLib.Logic;

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
    
    public async Task<Result<List<LocalWork>>> GetByUserIdAsync(string userId)
    {
        try
        {
            var localWorks = await _dbService.GetWorksByUserIdAsync(userId);
            var works = new List<LocalWork>();
            
            foreach (var localWork in localWorks)
            {
                var category = await _dbService.GetWorkCategoryByIdAsync(localWork.CategoryId);
                localWork.CategoryCode = category?.Code ?? "";
                
                var status = await _dbService.GetWorkStatusByIdAsync(localWork.StatusId);
                localWork.StatusCode = status?.Code ?? "";
                
                works.Add(localWork);
            }

            return Result<List<LocalWork>>.Success(works);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get works for user {UserId}", userId);
            return Result<List<LocalWork>>.Failure("Failed to load works", ex);
        }
    }

    public async Task<Result<LocalWork>> GetByIdAsync(string workId)
    {
        try
        {
            var localWork = await _dbService.GetByIdAsync<LocalWork>(workId);
            
            if (localWork == null)
            {
                return Result<LocalWork>.Failure("Work not found");
            }
            
            var category = await _dbService.GetWorkCategoryByIdAsync(localWork.CategoryId);
            localWork.CategoryCode = category?.Code ?? "";
            
            var status = await _dbService.GetWorkStatusByIdAsync(localWork.StatusId);
            localWork.StatusCode = status?.Code ?? "";
            
            return Result<LocalWork>.Success(localWork);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get work {WorkId}", workId);
            return Result<LocalWork>.Failure("Failed to load work", ex);
        }
    }

    public async Task<Result<LocalWork>> CreateAsync(LocalWork work, string userInitials)
    {
        try
        {
            var category = await _dbService.GetWorkCategoryByIdAsync(work.CategoryId);
            var categoryCode = category?.Code ?? "OTH";
            
            var existingWorks = await _dbService.GetWorksByUserIdAsync(work.UserId);
            if (string.IsNullOrEmpty(work.Code))
            {
                work.Code = CommonLogic.GenerateWorkCode(userInitials, categoryCode, existingWorks);
            }

            work.StatusId = (int)WorkStatusCode.Wet;
            work.CreatedAt = DateTime.UtcNow;
            work.UpdatedAt = DateTime.UtcNow;
            work.SyncStatus = SyncStatus.Pending.Code();

            await _dbService.InsertAsync(work);

            _logger.LogInformation("Work created with code: {WorkCode}", work.Code);
            return Result<LocalWork>.Success(work);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create work");
            return Result<LocalWork>.Failure("Failed to create work", ex);
        }
    }

    public async Task<Result> UpdateAsync(LocalWork work)
    {
        try
        {
            work.UpdatedAt = DateTime.UtcNow;
            work.SyncStatus = SyncStatus.Pending.Code();

            await _dbService.UpdateAsync(work);

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
    
    public async Task<Result<List<LocalWorkCategory>>> GetCategoriesAsync()
    {
        try
        {
            var categories = await _dbService.GetAllAsync<LocalWorkCategory>();
            return Result<List<LocalWorkCategory>>.Success(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get categories");
            return Result<List<LocalWorkCategory>>.Failure("Failed to load categories", ex);
        }
    }
    
    public async Task<Result<List<LocalWorkStatus>>> GetStatusesAsync()
    {
        try
        {
            var statuses = await _dbService.GetAllAsync<LocalWorkStatus>();
            return Result<List<LocalWorkStatus>>.Success(statuses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get statuses");
            return Result<List<LocalWorkStatus>>.Failure("Failed to load statuses", ex);
        }
    }
}
