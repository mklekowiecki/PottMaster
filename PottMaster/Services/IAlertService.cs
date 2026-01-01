using System.Threading.Tasks;

namespace PottMaster.Services;

public interface IAlertService
{
    Task ShowAlertAsync(string title, string message, string ok = "OK");
    Task<bool> ShowConfirmationAsync(string title, string message, string accept = "OK", string cancel = "Cancel");
}