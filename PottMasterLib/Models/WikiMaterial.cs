using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PottMasterLib.Models;

[Table("wiki_materials")]
public class WikiMaterial : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("type_id")]
    public int TypeId { get; set; }

    [Column("properties")]
    public string? Properties { get; set; } // JSON string or something

    [Column("trust_level")]
    public string TrustLevel { get; set; } = "unverified"; // verified, unverified, expert

    [Column("submitted_by")]
    public string? SubmittedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public WikiMaterialType? Type { get; set; }
}