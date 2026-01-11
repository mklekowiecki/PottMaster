using PottMasterLib.Models;

namespace PottMaster.Services;

public interface IWikiService
{
    Task<List<WikiMaterial>> SearchMaterialsAsync(string query);
    Task<List<WikiMaterial>> GetMaterialsByTypeAsync(int typeId);
    Task<List<WikiMaterialType>> GetMaterialTypesAsync();
    Task<List<WikiMaterial>> GetAllMaterialsAsync();
    Task<WikiMaterial?> GetMaterialByIdAsync(Guid id);
    Task SubmitMaterialAsync(WikiMaterial material);
    Task<List<WikiMaterial>> GetUnverifiedMaterialsAsync(); // For experts
    Task VerifyMaterialAsync(Guid id);
}