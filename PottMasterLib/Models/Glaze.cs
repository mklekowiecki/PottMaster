using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PottMasterLib.Models;

[Table("glazes")]
public class Glaze : BaseModel, IGlaze
{
    [PrimaryKey("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("manufacturer")]
    public string? Manufacturer { get; set; }

    [Column("batch_date")]
    public DateTime? BatchDate { get; set; }

    [Column("type_id")]
    public int? TypeId { get; set; }

    [Column("color")]
    public string? Color { get; set; }

    [Column("cone_rating")]
    public string? ConeRating { get; set; }

    [Column("quantity")]
    public string? Quantity { get; set; }

    [Column("properties")]
    public string PropertiesJson { get; set; } = "{}";

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("food_safe")]
    public bool? FoodSafe { get; set; }

    [Column("is_favorite")]
    public bool IsFavorite { get; set; }

    [Column("sync_status")]
    public string SyncStatus { get; set; } = "SYNCED";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}