namespace PottMasterLib.Models;

public interface IGlaze
{
    string Id { get; set; }
    string UserId { get; set; }
    string Name { get; set; }
    string? Manufacturer { get; set; }
    DateTime? BatchDate { get; set; }
    int? TypeId { get; set; }
    string? Color { get; set; }
    string? ConeRating { get; set; }
    string? Quantity { get; set; }
    string PropertiesJson { get; set; }
    string? Notes { get; set; }
    bool? FoodSafe { get; set; }
    bool IsFavorite { get; set; }
    string SyncStatus { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}