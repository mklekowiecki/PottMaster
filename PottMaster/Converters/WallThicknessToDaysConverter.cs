using PottMaster.Resources;
using System.Globalization;
using System.Resources;

namespace PottMaster.Converters;

public class WallThicknessToDaysConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double wallThickness)
        {
            var days = wallThickness switch
            {
                <= 5 => 4,
                <= 10 => 7,
                <= 15 => 10,
                _ => 14
            };
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
