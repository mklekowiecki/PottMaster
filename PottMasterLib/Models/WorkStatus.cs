using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PottMasterLib.Models;

public enum WorkStatusCode
{
    Wet = 1,
    LeatherHard = 2,
    BoneDry = 3,
    BisqueFired = 4,
    Glazed = 5,
    GlazeFired = 6,
    Completed = 7,
    Discarded = 8
}

[Table("work_statuses")]
public class WorkStatus : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("code")]
    public string Code { get; set; } = string.Empty;
}
