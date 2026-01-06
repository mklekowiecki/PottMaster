using PottMaster.Repositories;
using PottMasterLib.Models;

namespace PottMaster.Services;

public class GlazeService : IGlazeService
{
    private readonly IGlazeRepository _repository;

    public GlazeService(IGlazeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LocalGlaze>> GetUserGlazesAsync(string userId)
    {
        return await _repository.GetUserGlazesAsync(userId);
    }

    public async Task<LocalGlaze?> GetGlazeByIdAsync(string glazeId)
    {
        return await _repository.GetGlazeByIdAsync(glazeId);
    }

    public async Task<LocalGlaze> CreateGlazeAsync(LocalGlaze glaze)
    {
        if (string.IsNullOrWhiteSpace(glaze.Name))
        {
            throw new ArgumentException("Glaze name is required", nameof(glaze.Name));
        }

        return await _repository.CreateGlazeAsync(glaze);
    }

    public async Task UpdateGlazeAsync(LocalGlaze glaze)
    {
        if (string.IsNullOrWhiteSpace(glaze.Name))
        {
            throw new ArgumentException("Glaze name is required", nameof(glaze.Name));
        }

        await _repository.UpdateGlazeAsync(glaze);
    }

    public async Task DeleteGlazeAsync(string glazeId)
    {
        await _repository.DeleteGlazeAsync(glazeId);
    }

    public async Task<List<LocalGlazeType>> GetGlazeTypesAsync()
    {
        var types = await _repository.GetGlazeTypesAsync();
        return types;
    }
}
