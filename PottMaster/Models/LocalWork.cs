using SQLite;

namespace PottMaster.Models;

[Table("works")]
public class LocalWork
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Indexed]
    public string UserId { get; set; } = string.Empty;

    [Unique]
    public string Code { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int WallThickness { get; set; }

    public string? PhotoPath { get; set; }

    public int StatusId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DryingStartedAt { get; set; }

    public DateTime? DryingCompletedAt { get; set; }

    public string SyncStatus { get; set; } = "PENDING";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Additional properties for mapping
    [Ignore]
    public string CategoryCode { get; set; } = string.Empty;

    [Ignore]
    public string StatusCode { get; set; } = string.Empty;
}