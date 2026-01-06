namespace PottMasterLib.Models;

public class LocalGlazeType : IGlazeType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
