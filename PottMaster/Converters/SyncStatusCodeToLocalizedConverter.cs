using System.Globalization;
using System.Resources;
using PottMaster.Resources;

namespace PottMaster.Converters;

public class SyncStatusCodeToLocalizedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string syncStatusCode)
        {
            return syncStatusCode switch
            {
                "PENDING" => AppResources.SyncStatusPending,
                "SYNCING" => AppResources.SyncStatusSyncing,
                "SYNCED" => AppResources.SyncStatusSynced,
                "ERROR" => AppResources.SyncStatusError,
                _ => AppResources.SyncStatusUnknown
            };
        }

        return AppResources.SyncStatusUnknown;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}