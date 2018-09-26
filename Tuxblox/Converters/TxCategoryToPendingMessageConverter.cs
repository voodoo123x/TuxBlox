using System;
using System.Globalization;
using System.Windows.Data;
using Tuxblox.Model.Entities;

namespace Tuxblox.Converters
{
    public class TxCategoryToPendingMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pendingMessage = string.Empty;

            var txCategory = (TxCategory)value;
            switch (txCategory)
            {
                case TxCategory.Generate:
                case TxCategory.Immature:
                    pendingMessage = " (IMMATURE)";
                    break;

                default:
                    pendingMessage = " (PENDING)";
                    break;
            }

            return pendingMessage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
