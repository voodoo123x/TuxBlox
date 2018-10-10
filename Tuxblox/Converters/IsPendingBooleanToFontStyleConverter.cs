using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tuxblox.Converters
{
    public class IsPendingBooleanToFontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isPending = (bool)value;
            if (isPending)
            {
                return FontStyles.Italic;
            }
            else
            {
                return FontStyles.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
