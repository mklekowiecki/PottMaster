using PottMaster.Models;
using PottMaster.Repositories;

namespace PottMaster.Services;

public class WorkService : IWorkService
{
    private readonly IWorkRepository _workRepository;
    private readonly IErrorHandlingService _errorHandler;

    public WorkService(IWorkRepository workRepository, IErrorHandlingService errorHandler)
    {
        _workRepository = workRepository;
        _errorHandler = errorHandler;
    }

    public async Task<List<Work>> GetUserWorksAsync(string userId)
    {
        var result = await _workRepository.GetByUserIdAsync(userId);
        
        if (!result.IsSuccess)
        {
            if (result.Exception != null)
            {
                await _errorHandler.HandleErrorAsync(result.Exception, nameof(GetUserWorksAsync), false);
            }
            return new List<Work>();
        }
        
        return result.Value ?? new List<Work>();
    }

    public async Task<Work?> GetWorkByIdAsync(string workId)
    {
        var result = await _workRepository.GetByIdAsync(workId);
        
        if (!result.IsSuccess)
        {
            if (result.Exception != null)
            {
                await _errorHandler.HandleErrorAsync(result.Exception, nameof(GetWorkByIdAsync), false);
            }
            return null;
        }
        
        return result.Value;
    }

    public async Task<string> CreateWorkAsync(Work work, string userInitials)
    {
        var result = await _workRepository.CreateAsync(work, userInitials);
        
        if (!result.IsSuccess)
        {
            if (result.Exception != null)
            {
                await _errorHandler.HandleErrorAsync(result.Exception, nameof(CreateWorkAsync));
            }
            throw new InvalidOperationException(result.Error ?? "Failed to create work");
        }
        
        return result.Value ?? string.Empty;
    }

    public async Task UpdateWorkAsync(Work work)
    {
        var result = await _workRepository.UpdateAsync(work);
        
        if (!result.IsSuccess)
        {
            if (result.Exception != null)
            {
                await _errorHandler.HandleErrorAsync(result.Exception, nameof(UpdateWorkAsync));
            }
            throw new InvalidOperationException(result.Error ?? "Failed to update work");
        }
    }

    public async Task DeleteWorkAsync(string workId)
    {
        var result = await _workRepository.DeleteAsync(workId);
        
        if (!result.IsSuccess)
        {
            if (result.Exception != null)
            {
                await _errorHandler.HandleErrorAsync(result.Exception, nameof(DeleteWorkAsync));
            }
            throw new InvalidOperationException(result.Error ?? "Failed to delete work");
        }
    }

    public async Task<List<WorkCategory>> GetCategoriesAsync()
    {
        var result = await _workRepository.GetCategoriesAsync();
        
        if (!result.IsSuccess)
        {
            if (result.Exception != null)
            {
                await _errorHandler.HandleErrorAsync(result.Exception, nameof(GetCategoriesAsync), false);
            }
            return new List<WorkCategory>();
        }
        
        return result.Value ?? new List<WorkCategory>();
    }

    public async Task<List<WorkStatus>> GetStatusesAsync()
    {
        var result = await _workRepository.GetStatusesAsync();
        
        if (!result.IsSuccess)
        {
            if (result.Exception != null)
            {
                await _errorHandler.HandleErrorAsync(result.Exception, nameof(GetStatusesAsync), false);
            }
            return new List<WorkStatus>();
        }
        
        return result.Value ?? new List<WorkStatus>();
    }
}
