namespace PottMaster.Services;

public interface IDryingMonitorService
{
    Task CheckAndAdvanceCompletedWorksAsync();
}