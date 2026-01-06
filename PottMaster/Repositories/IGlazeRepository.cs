using PottMasterLib.Models;

namespace PottMaster.Repositories;

public interface IGlazeRepository
{
    Task<List<LocalGlaze>> GetUserGlazesAsync(string userId);
    Task<LocalGlaze?> GetGlazeByIdAsync(string glazeId);
    Task<LocalGlaze> CreateGlazeAsync(LocalGlaze glaze);
    Task UpdateGlazeAsync(LocalGlaze glaze);
    Task DeleteGlazeAsync(string glazeId);
    Task<List<LocalGlaze>> GetFavoriteGlazesAsync(string userId);
    Task<List<LocalGlazeType>> GetGlazeTypesAsync();
    Task<int> UpsertGlazeTypesAsync(List<LocalGlazeType> types);
}
