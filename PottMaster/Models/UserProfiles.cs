using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace PottMaster.Models;

[Table("user_profiles")]
public class UserProfiles : BaseModel
{
    [Column("id")]
    public string Id { get; set; } = string.Empty;

    [Column("initials")]
    public string Initials { get; set; } = string.Empty;
}