using PottMasterLib.Models;

namespace PottMaster.Services;

public interface IWikiService
{
    Task<List<WikiMaterial>> SearchMaterialsAsync(string query);
    Task<List<WikiMaterial>> GetMaterialsByTypeAsync(int typeId);
    Task<List<WikiMaterialType>> GetMaterialTypesAsync();
    Task<WikiMaterial?> GetMaterialByIdAsync(int id);
    Task SubmitMaterialAsync(WikiMaterial material);
    Task<List<WikiMaterial>> GetUnverifiedMaterialsAsync(); // For experts
    Task VerifyMaterialAsync(int id);
}