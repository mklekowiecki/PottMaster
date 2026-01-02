using System.Threading.Tasks;

namespace PottMaster.Services;

public class AlertService : IAlertService
{
    public async Task ShowAlertAsync(string title, string message, string ok = "OK")
    {
        if (Shell.Current != null)
        {
            await Shell.Current.DisplayAlert(title, message, ok);
        }
        else if (Application.Current?.Windows[0] != null)
        {
            await Application.Current.Windows[0].Page!.DisplayAlert(title, message, ok);
        }
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "OK", string cancel = "Cancel")
    {
        if (Shell.Current != null)
        {
            return await Shell.Current.DisplayAlert(title, message, accept, cancel);
        }
        else if (Application.Current?.Windows[0] != null)
        {
            return await Application.Current.Windows[0].Page!.DisplayAlert(title, message, accept, cancel);
        }

        // Default to false if no UI available
        return false;
    }
}