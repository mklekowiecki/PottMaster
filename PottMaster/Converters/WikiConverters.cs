using System.Globalization;
using Microsoft.Maui.Controls;

namespace PottMaster.Converters;

public class TrustLevelToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string trustLevel)
        {
            return trustLevel.ToLower() switch
            {
                "verified" => Color.FromArgb("#4CAF50"), // Green
                "expert" => Color.FromArgb("#FF9800"), // Orange
                "unverified" => Color.FromArgb("#9E9E9E"), // Gray
                _ => Color.FromArgb("#9E9E9E")
            };
        }
        return Color.FromArgb("#9E9E9E");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}