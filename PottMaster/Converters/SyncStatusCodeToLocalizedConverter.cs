using PottMaster.Resources;
using PottMasterLib.Models;
using System.Globalization;
using System.Resources;

namespace PottMaster.Converters;

public class SyncStatusCodeToLocalizedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string syncStatusCode)
        {
            // Convert code to enum
            var status = Helpers.EnumHelpers.CodeToEnum<SyncStatus>(syncStatusCode);

            if (status.HasValue)
            {
                return status.Value switch
                {
                    SyncStatus.Pending => AppResources.SyncStatusPending,
                    SyncStatus.Syncing => AppResources.SyncStatusSyncing,
                    SyncStatus.Synced => AppResources.SyncStatusSynced,
                    SyncStatus.Error => AppResources.SyncStatusError,
                    _ => AppResources.SyncStatusUnknown
                };
            }
        }

        return AppResources.SyncStatusUnknown;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}