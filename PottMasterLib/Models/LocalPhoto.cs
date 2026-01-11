using SQLite;

namespace PottMasterLib.Models;

[Table("photos")]
public class LocalPhoto
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Indexed]
    public string WorkId { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string? RemotePath { get; set; }

    public int Order { get; set; }

    public string SyncStatus { get; set; } = "PENDING";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}