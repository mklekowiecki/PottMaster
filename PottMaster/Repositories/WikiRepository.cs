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
            .Select("*, MaterialType:wiki_material_types(*)")
            .Filter("name", Supabase.Postgrest.Constants.Operator.ILike, $"%{query}%")
            .Get();

        return response.Models;
    }

    public async Task<List<WikiMaterial>> GetMaterialsByTypeAsync(int typeId)
    {
        var response = await _supabaseClient
            .From<WikiMaterial>()
            .Select("*, MaterialType:wiki_material_types(*)")
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

    public async Task<WikiMaterial?> GetMaterialByIdAsync(Guid id)
    {
        var response = await _supabaseClient
            .From<WikiMaterial>()
            .Select("*, MaterialType:wiki_material_types(*)")
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Get();

        return response.Models.FirstOrDefault();
    }

	public async Task SubmitMaterialAsync(WikiMaterial material)
	{
		Console.WriteLine($"Submitting material with SubmittedBy: {material.SubmittedBy}");
		await _supabaseClient
			.From<WikiMaterial>()
			.Insert(material);
	}

	public async Task<List<WikiMaterial>> GetUnverifiedMaterialsAsync()
    {
        var response = await _supabaseClient
            .From<WikiMaterial>()
            .Select("*, MaterialType:wiki_material_types(*)")
            .Filter("trust_level", Supabase.Postgrest.Constants.Operator.Equals, "unverified")
            .Get();

        return response.Models;
    }

    public async Task VerifyMaterialAsync(Guid id)
    {
        await _supabaseClient
            .From<WikiMaterial>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Set(x => x.TrustLevel, "verified")
            .Update();
    }

    public async Task<List<WikiMaterial>> GetAllMaterialsAsync()
    {
        var response = await _supabaseClient
            .From<WikiMaterial>()
            .Select("*, MaterialType:wiki_material_types(*)")
            .Get();

        return response.Models;
    }
}