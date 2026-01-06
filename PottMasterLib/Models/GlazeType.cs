using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PottMasterLib.Models;

[Table("glaze_types")]
public class GlazeType : BaseModel, IGlazeType
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("code")]
    public string Code { get; set; } = string.Empty;
}