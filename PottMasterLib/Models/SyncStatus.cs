using System.Reflection;

namespace PottMasterLib.Models;

public enum SyncStatus
{
    [Code("PENDING")]
    Pending,
    [Code("SYNCING")]
    Syncing,
    [Code("SYNCED")]
    Synced,
    [Code("ERROR")]
    Error
}

public static class SyncStatusExtensions
{
    public static string Code(this SyncStatus status)
    {
        var field = status.GetType().GetField(status.ToString());
        var attribute = field?.GetCustomAttribute<CodeAttribute>();
        return attribute?.Code ?? status.ToString();
    }
}