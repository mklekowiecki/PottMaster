using System.Globalization;

namespace PottMaster.Converters;

public class BoolToTitleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isEditMode && parameter is string param)
        {
            var parts = param.Split('|');
            return isEditMode ? parts[1] : parts[0];
        }
        return parameter?.ToString()?.Split('|')[0] ?? "New Glaze";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
