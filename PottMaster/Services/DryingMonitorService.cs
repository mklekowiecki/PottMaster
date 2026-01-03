using PottMasterLib.Services;
using PottMasterLib.Models;
using PottMasterLib.Logic;
using static PottMasterLib.Models.WorkStatusCode;
using static PottMasterLib.Models.SyncStatus;

namespace PottMaster.Services;

public class DryingMonitorService : IDryingMonitorService
{
    private readonly IDbService _dbService;
    private readonly IWorkService _workService;
    private readonly INotificationService _notificationService;

    public DryingMonitorService(IDbService dbService, IWorkService workService, INotificationService notificationService)
    {
        _dbService = dbService;
        _workService = workService;
        _notificationService = notificationService;
    }

    public async Task CheckAndAdvanceCompletedWorksAsync()
    {
        var works = await _dbService.GetAllAsync<LocalWork>();
        var dryingWorks = works.Where(w => w.StatusId == (int)Wet || w.StatusId == (int)LeatherHard);

        foreach (var work in dryingWorks)
        {
            var remainingTime = CalculateRemainingDryingTime(work);
            if (remainingTime <= TimeSpan.Zero)
            {
                // Advance to Bone Dry status
                work.StatusId = (int)BoneDry;
                work.DryingCompletedAt = DateTime.UtcNow;
                work.UpdatedAt = DateTime.UtcNow;
                work.SyncStatus = Pending.Code();

                await _dbService.UpdateAsync(work);

                // Send notification
                await _notificationService.SendDryingCompleteNotificationAsync(work);
            }
        }
    }

    // Business rule:
        // We approximate drying duration based on wall thickness so that works reach a safe "bone dry"
        // state before firing. These thresholds are derived from studio practice and include a safety
        // buffer to reduce the risk of cracking or explosions in the kiln:
        // - ≤ 5 mm walls: 4 days (very thin pieces dry quickly).
        // - ≤ 10 mm walls: 7 days (standard thickness, needs about a week).
        // - ≤ 15 mm walls: 10 days (thicker pieces require extra time).
        // - > 15 mm walls: 14 days (very thick / heavy pieces get the maximum drying time).
        // If the studio's guidelines change, update this mapping accordingly.
    private TimeSpan CalculateRemainingDryingTime(LocalWork work)
    {
        var startTime = work.DryingStartedAt ?? work.CreatedAt;
        var dryingDays = CommonLogic.CalculateDryingDays(work.WallThickness);

        var targetDate = startTime.AddDays(dryingDays);
        return targetDate - DateTime.UtcNow;
    }
}