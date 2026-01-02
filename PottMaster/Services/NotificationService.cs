using PottMaster.Resources;
using PottMasterLib.Models;
using System.Globalization;
using System.Resources;

namespace PottMaster.Services;

public class NotificationService : INotificationService
{
    private readonly IAlertService _alertService;
    private readonly ResourceManager _resourceManager;

    public NotificationService(IAlertService alertService)
    {
        _alertService = alertService;
        _resourceManager = AppResources.ResourceManager;
    }

    public async Task SendDryingCompleteNotificationAsync(LocalWork work)
    {
        string title = _resourceManager.GetString("DryingCompleteInfo")! ;
        string messageTemplate = _resourceManager.GetString("DryingCompleteMessage")!;
        string message = string.Format(
            messageTemplate,
            work.Code
        );

        await _alertService.ShowAlertAsync(title, message);
    }
}