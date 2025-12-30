using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace PottMaster.Models;

public class UserProfile : BaseModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("initials")]
    public string Initials { get; set; } = string.Empty;
}