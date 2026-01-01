using System.Globalization;
using PottMaster.Resources;

namespace PottMaster.Converters;

public class WorkStatusCodeToLocalizedConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string code || string.IsNullOrWhiteSpace(code))
            return string.Empty;

        var resourceKey = $"WorkStatus_{code}";
        var resourceManager = AppResources.ResourceManager;
        
        try
        {
            var localizedValue = resourceManager.GetString(resourceKey, culture);
            return localizedValue ?? code;
        }
        catch
        {
            return code;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
