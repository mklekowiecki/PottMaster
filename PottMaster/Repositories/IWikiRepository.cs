using PottMasterLib.Models;

namespace PottMaster.Repositories;

public interface IWikiRepository
{
    Task<List<WikiMaterial>> SearchMaterialsAsync(string query);
    Task<List<WikiMaterial>> GetMaterialsByTypeAsync(int typeId);
    Task<List<WikiMaterialType>> GetMaterialTypesAsync();
    Task<WikiMaterial?> GetMaterialByIdAsync(int id);
    Task SubmitMaterialAsync(WikiMaterial material);
    Task<List<WikiMaterial>> GetUnverifiedMaterialsAsync();
    Task VerifyMaterialAsync(int id);
}