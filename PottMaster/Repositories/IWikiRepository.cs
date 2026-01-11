using PottMasterLib.Models;

namespace PottMaster.Repositories;

public interface IWikiRepository
{
    Task<List<WikiMaterial>> SearchMaterialsAsync(string query);
    Task<List<WikiMaterial>> GetMaterialsByTypeAsync(int typeId);
    Task<List<WikiMaterialType>> GetMaterialTypesAsync();
    Task<List<WikiMaterial>> GetAllMaterialsAsync();
    Task<WikiMaterial?> GetMaterialByIdAsync(Guid id);
    Task SubmitMaterialAsync(WikiMaterial material);
    Task<List<WikiMaterial>> GetUnverifiedMaterialsAsync();
    Task VerifyMaterialAsync(Guid id);
}