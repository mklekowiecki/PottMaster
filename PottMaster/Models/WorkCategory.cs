using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PottMaster.Models;

[Table("work_categories")]
public class WorkCategory : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("code")]
    public string Code { get; set; } = string.Empty;
}
