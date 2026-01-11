using PottMasterLib.Models;
using Supabase;

namespace PottMaster.Repositories;

public class WikiRepository : IWikiRepository
{
    private readonly Supabase.Client _supabaseClient;

    public WikiRepository(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<List<WikiMaterial>> SearchMaterialsAsync(string query)
    {
        var response = await _supabaseClient
            .From<WikiMaterial>()
            .Select("*")
            .Filter("name", Supabase.Postgrest.Constants.Operator.ILike, $"%{query}%")
            .Get();

        return response.Models;
    }

    public async Task<List<WikiMaterial>> GetMaterialsByTypeAsync(int typeId)
    {
        var response = await _supabaseClient
            .From<WikiMaterial>()
            .Select("*")
            .Filter("type_id", Supabase.Postgrest.Constants.Operator.Equals, typeId)
            .Get();

        return response.Models;
    }

    public async Task<List<WikiMaterialType>> GetMaterialTypesAsync()
    {
        var response = await _supabaseClient
            .From<WikiMaterialType>()
            .Select("*")
            .Get();

        return response.Models;
    }

    public async Task<WikiMaterial?> GetMaterialByIdAsync(int id)
    {
        var response = await _supabaseClient
            .From<WikiMaterial>()
            .Select("*")
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task SubmitMaterialAsync(WikiMaterial material)
    {
        await _supabaseClient
            .From<WikiMaterial>()
            .Insert(material);
    }

    public async Task<List<WikiMaterial>> GetUnverifiedMaterialsAsync()
    {
        var response = await _supabaseClient
            .From<WikiMaterial>()
            .Select("*")
            .Filter("trust_level", Supabase.Postgrest.Constants.Operator.Equals, "unverified")
            .Get();

        return response.Models;
    }

    public async Task VerifyMaterialAsync(int id)
    {
        await _supabaseClient
            .From<WikiMaterial>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Set(x => x.TrustLevel, "verified")
            .Update();
    }
}