using PottMasterLib.Models;
using PottMasterLib.Services;

namespace PottMaster.Repositories;

public class LocalGlazeRepository : IGlazeRepository
{
    private readonly IDbService _dbService;

    public LocalGlazeRepository(IDbService dbService)
    {
        _dbService = dbService;
    }

    public async Task<List<LocalGlaze>> GetUserGlazesAsync(string userId)
    {
        var allGlazes = await _dbService.GetAllAsync<LocalGlaze>();
        return allGlazes.Where(g => g.UserId == userId)
                       .OrderByDescending(g => g.IsFavorite)
                       .ThenBy(g => g.Name)
                       .ToList();
    }

    public async Task<LocalGlaze?> GetGlazeByIdAsync(string glazeId)
    {
        return await _dbService.GetByIdAsync<LocalGlaze>(glazeId);
    }

    public async Task<LocalGlaze> CreateGlazeAsync(LocalGlaze glaze)
    {
        glaze.Id = Guid.NewGuid().ToString();
        glaze.CreatedAt = DateTime.UtcNow;
        glaze.UpdatedAt = DateTime.UtcNow;
        glaze.SyncStatus = SyncStatus.Pending.Code();
        
        await _dbService.InsertAsync(glaze);
        return glaze;
    }

    public async Task UpdateGlazeAsync(LocalGlaze glaze)
    {
        glaze.UpdatedAt = DateTime.UtcNow;
        glaze.SyncStatus = SyncStatus.Pending.Code();
        await _dbService.UpdateAsync(glaze);
    }

    public async Task DeleteGlazeAsync(string glazeId)
    {
        var glaze = await GetGlazeByIdAsync(glazeId);
        if (glaze != null)
        {
            await _dbService.DeleteAsync(glaze);
        }
    }

    public async Task<List<LocalGlaze>> GetFavoriteGlazesAsync(string userId)
    {
        var allGlazes = await GetUserGlazesAsync(userId);
        return allGlazes.Where(g => g.IsFavorite).ToList();
    }

    public async Task<List<LocalGlazeType>> GetGlazeTypesAsync()
    {
        return await _dbService.GetAllAsync<LocalGlazeType>();
    }

    public async Task<int> UpsertGlazeTypesAsync(List<LocalGlazeType> types)
    {
        return await _dbService.UpsertAllAsync(types);
    }
}
