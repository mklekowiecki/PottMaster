using SQLite;

namespace PottMaster.Models;

[Table("work_statuses")]
public class WorkStatus
{
    [PrimaryKey]
    public int Id { get; set; }

    [Unique]
    public string Name { get; set; } = string.Empty;

    [Unique]
    public string Code { get; set; } = string.Empty;
}
