using PottMasterLib.Models;

namespace PottMaster.Services;

public interface IGlazeService
{
    Task<List<LocalGlaze>> GetUserGlazesAsync(string userId);
    Task<LocalGlaze?> GetGlazeByIdAsync(string glazeId);
    Task<LocalGlaze> CreateGlazeAsync(LocalGlaze glaze);
    Task UpdateGlazeAsync(LocalGlaze glaze);
    Task DeleteGlazeAsync(string glazeId);
    Task<List<LocalGlazeType>> GetGlazeTypesAsync();
}
