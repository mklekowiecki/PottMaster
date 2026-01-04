using System.Globalization;
using Microsoft.Maui.Controls;

namespace PottMaster.Converters;

public class FoodSafeToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool foodSafe)
        {
            return foodSafe ? "? Food Safe" : "? Not Food Safe";
        }
        return "? Not Tested";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class FoodSafeToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool foodSafe)
        {
            return foodSafe ? Colors.Green : Colors.Red;
        }
        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToStarIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isFavorite && isFavorite)
        {
            return "star_filled.png"; // You'll need to add these icons
        }
        return "star_outline.png";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class GlazeTypeIdToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int typeId)
        {
            return typeId switch
            {
                1 => Color.FromArgb("#FF6B6B"), // Low-fire - Red
                2 => Color.FromArgb("#4ECDC4"), // Mid-range - Teal
                3 => Color.FromArgb("#FFD93D"), // High-fire - Yellow
                _ => Colors.Gray
            };
        }
        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
