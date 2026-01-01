using Supabase.Postgrest.Attributes;

namespace PottMaster.Models;

[Supabase.Postgrest.Attributes.Table("wiki_material_types")]
public class WikiMaterialType
{
    [Supabase.Postgrest.Attributes.PrimaryKey("id")]
    public int Id { get; set; }

    [Supabase.Postgrest.Attributes.Column("name")]
    public string Name { get; set; } = string.Empty;
}