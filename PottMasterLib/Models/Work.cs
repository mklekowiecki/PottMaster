using SQLite;

namespace PottMasterLib.Models;

[Table("works")]
public class Work
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

    [Ignore]
    public string CategoryCode { get; set; } = string.Empty;

    [Ignore]
    public string StatusCode { get; set; } = string.Empty;

    [Ignore]
    public TimeSpan? RemainingDryingTime
    {
        get
        {
            if (StatusId >= (int)WorkStatusCode.BoneDry) return TimeSpan.Zero;

            var startTime = DryingStartedAt ?? CreatedAt;
            var dryingDays = WallThickness switch
            {
                <= 5 => 4,
                <= 10 => 7,
                <= 15 => 10,
                _ => 14
            };

            var targetDate = startTime.AddDays(dryingDays);
            var remaining = targetDate - DateTime.UtcNow;

            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }
}
