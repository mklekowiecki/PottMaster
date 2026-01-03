namespace PottMasterLib.Models;

public interface IWork
{
    string Id { get; set; }
    string UserId { get; set; }
    string Code { get; set; }
    int CategoryId { get; set; }
    int WallThickness { get; set; }
    string? PhotoPath { get; set; }
    int StatusId { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? DryingStartedAt { get; set; }
    DateTime? DryingCompletedAt { get; set; }
    string SyncStatus { get; set; }
    DateTime UpdatedAt { get; set; }
}
