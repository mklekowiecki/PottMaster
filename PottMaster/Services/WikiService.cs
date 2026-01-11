using PottMaster.Repositories;
using PottMasterLib.Models;

namespace PottMaster.Services;

public class WikiService : IWikiService
{
    private readonly IWikiRepository _repository;

    public WikiService(IWikiRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WikiMaterial>> SearchMaterialsAsync(string query)
    {
        return await _repository.SearchMaterialsAsync(query);
    }

    public async Task<List<WikiMaterial>> GetMaterialsByTypeAsync(int typeId)
    {
        return await _repository.GetMaterialsByTypeAsync(typeId);
    }

    public async Task<List<WikiMaterialType>> GetMaterialTypesAsync()
    {
        return await _repository.GetMaterialTypesAsync();
    }

    public async Task<WikiMaterial?> GetMaterialByIdAsync(int id)
    {
        return await _repository.GetMaterialByIdAsync(id);
    }

    public async Task SubmitMaterialAsync(WikiMaterial material)
    {
        await _repository.SubmitMaterialAsync(material);
    }

    public async Task<List<WikiMaterial>> GetUnverifiedMaterialsAsync()
    {
        return await _repository.GetUnverifiedMaterialsAsync();
    }

    public async Task VerifyMaterialAsync(int id)
    {
        await _repository.VerifyMaterialAsync(id);
    }
}