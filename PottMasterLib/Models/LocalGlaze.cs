using SQLite;
using System.Text.Json;
using PottMasterLib.Models.GlazeProperties;

namespace PottMasterLib.Models;

[Table("glazes")]
public class LocalGlaze
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Indexed]
    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Manufacturer { get; set; }

    public DateTime? BatchDate { get; set; }

    public int? TypeId { get; set; }

    public string? Color { get; set; }

    public string? ConeRating { get; set; }

    public string? Quantity { get; set; }

    public string PropertiesJson { get; set; } = "{}";

    public string? Notes { get; set; }

    public bool? FoodSafe { get; set; }

    public bool IsFavorite { get; set; }

    public string SyncStatus { get; set; } = "PENDING";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Ignore]
    public GlazePropertiesModel Properties
    {
        get
        {
            try
            {
                return string.IsNullOrWhiteSpace(PropertiesJson)
                    ? new GlazePropertiesModel()
                    : JsonSerializer.Deserialize<GlazePropertiesModel>(PropertiesJson) ?? new GlazePropertiesModel();
            }
            catch
            {
                return new GlazePropertiesModel();
            }
        }
        set
        {
            PropertiesJson = JsonSerializer.Serialize(value);
        }
    }

    [Ignore]
    public string TypeName { get; set; } = string.Empty;

    [Ignore]
    public string FoodSafeDisplay => FoodSafe switch
    {
        true => "? Food Safe",
        false => "? Not Food Safe",
        null => "? Not Tested"
    };

    [Ignore]
    public List<string> QuickTags
    {
        get
        {
            var tags = new List<string>();
            if (!string.IsNullOrEmpty(ConeRating)) tags.Add(ConeRating);
            if (!string.IsNullOrEmpty(TypeName)) tags.Add(TypeName);
            if (IsFavorite) tags.Add("? Favorite");
            if (Properties?.Appearance?.Finish != null) tags.Add(Properties.Appearance.Finish);
            return tags;
        }
    }
}
