using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;

namespace PottMaster.Models;

[Table("work_statuses")]
public class WorkStatus
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("code")]
    public string Code { get; set; } = string.Empty;
}
