using System.Globalization;

namespace DreamAlchemist.Helpers.Converters;

public class PriceChangeToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal changePercent)
        {
            if (changePercent > 0)
                return Color.FromArgb("#10B981"); // Green for positive
            else if (changePercent < 0)
                return Color.FromArgb("#EF4444"); // Red for negative
            else
                return Color.FromArgb("#9CA3AF"); // Gray for neutral
        }
        return Color.FromArgb("#9CA3AF");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
