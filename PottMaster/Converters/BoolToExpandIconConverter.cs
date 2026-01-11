using System.Globalization;

namespace PottMaster.Converters;

public class BoolToExpandIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isExpanded && isExpanded)
        {
            return "?"; // Minus sign for expanded
        }
        return "+"; // Plus sign for collapsed
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
