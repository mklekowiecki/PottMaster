using SQLite;

namespace PottMasterLib.Models;

[Table("work_categories")]
public class LocalWorkCategory
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}