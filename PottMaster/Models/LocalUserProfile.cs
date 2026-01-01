using SQLite;

namespace PottMaster.Models;

[Table("user_profiles")]
public class LocalUserProfile
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Initials { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Preferences { get; set; } = "{}";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
