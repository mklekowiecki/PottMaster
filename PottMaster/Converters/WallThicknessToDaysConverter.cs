using PottMaster.Resources;
using PottMasterLib.Logic;
using System.Globalization;
using System.Resources;

namespace PottMaster.Converters;

public class WallThicknessToDaysConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double wallThickness)
        {
            var days = CommonLogic.CalculateDryingDays((int)wallThickness);
            var resourceManager = AppResources.ResourceManager;
            var localizedValue = resourceManager.GetString("DryingDays", culture);

            return $"{days} {localizedValue}";
        }

        return "0 days";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
