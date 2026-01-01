using SQLite;

namespace PottMaster.Models;

[Table("work_categories")]
public class WorkCategory
{
    [PrimaryKey]
    public int Id { get; set; }

    [Unique]
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}
