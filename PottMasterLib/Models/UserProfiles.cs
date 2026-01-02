using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace PottMasterLib.Models;

[Table("user_profiles")]
public class UserProfiles : BaseModel
{
    [Column("id")]
    public string Id { get; set; } = string.Empty;

    [Column("initials")]
    public string Initials { get; set; } = string.Empty;

	[Column("email")]
	public string Email { get; internal set; } = string.Empty;

	[Column("created_at")]
	public DateTime CreatedAt { get; internal set; }

}