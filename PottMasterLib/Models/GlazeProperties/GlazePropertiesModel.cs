namespace PottMasterLib.Models.GlazeProperties;

public class GlazePropertiesModel
{
    public FiringProperties? Firing { get; set; }
    public AppearanceProperties? Appearance { get; set; }
    public BehaviorProperties? Behavior { get; set; }
    public ApplicationProperties? Application { get; set; }
    public ClayCompatibilityProperties? ClayCompatibility { get; set; }
    public DefectsProperties? Defects { get; set; }
    public UsageProperties? Usage { get; set; }
}

public class FiringProperties
{
    public int? TemperatureMin { get; set; }
    public int? TemperatureMax { get; set; }
    public string TemperatureUnit { get; set; } = "C";
    public string? Atmosphere { get; set; }
    public string? CurveSensitivity { get; set; }

    public string TemperatureRange => TemperatureMin.HasValue && TemperatureMax.HasValue
        ? $"{TemperatureMin}°{TemperatureUnit} - {TemperatureMax}°{TemperatureUnit}"
        : string.Empty;
}

public class AppearanceProperties
{
    public string? Transparency { get; set; }
    public string? Finish { get; set; }
    public List<string> Texture { get; set; } = new();
    public List<string> SpecialEffects { get; set; } = new();

    public List<string> QuickTags
    {
        get
        {
            var tags = new List<string>();
            if (!string.IsNullOrEmpty(Transparency)) tags.Add(Transparency);
            if (!string.IsNullOrEmpty(Finish)) tags.Add(Finish);
            tags.AddRange(Texture);
            tags.AddRange(SpecialEffects);
            return tags;
        }
    }
}

public class BehaviorProperties
{
    public string? MeltFluidity { get; set; }
    public string? ThicknessTolerance { get; set; }
    public string? ColorStability { get; set; }
    public string? Repeatability { get; set; }
}

public class ApplicationProperties
{
    public string? Form { get; set; }
    public List<string> Methods { get; set; } = new();
    public string? RecommendedThickness { get; set; }
    public string? ApplicationNotes { get; set; }
}

public class ClayCompatibilityProperties
{
    public List<string> BestSuited { get; set; } = new();
    public string? Interaction { get; set; }
}

public class DefectsProperties
{
    public List<string> KnownIssues { get; set; } = new();
    public string? MitigationNotes { get; set; }
}

public class UsageProperties
{
    public string? WorkType { get; set; }
    public string? Durability { get; set; }
}
