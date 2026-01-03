using SQLite;

namespace PottMasterLib.Models;

[Table("work_statuses")]
public class LocalWorkStatus : IWorkStatus
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}