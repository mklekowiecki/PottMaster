using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PottMasterLib.Models;

[Table("works")]
public class RemoteWork : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;

    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("wall_thickness")]
    public int WallThickness { get; set; }

    [Column("photo_path")]
    public string? PhotoPath { get; set; }

    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("drying_started_at")]
    public DateTime? DryingStartedAt { get; set; }

    [Column("drying_completed_at")]
    public DateTime? DryingCompletedAt { get; set; }

    [Column("sync_status")]
    public string SyncStatus { get; set; } = "PENDING";

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
