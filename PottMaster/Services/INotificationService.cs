using PottMasterLib.Models;

namespace PottMaster.Services;

public interface INotificationService
{
    Task SendDryingCompleteNotificationAsync(LocalWork work);
}