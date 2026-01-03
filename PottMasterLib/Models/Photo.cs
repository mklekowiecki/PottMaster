using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PottMasterLib.Models;

[Table("photos")]
public class Photo : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;

    [Column("work_id")]
    public string WorkId { get; set; } = string.Empty;

    [Column("remote_path")]
    public string RemotePath { get; set; } = string.Empty;

    [Column("order")]
    public int Order { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}