using System;
using System.Globalization;
using System.Windows.Data;

namespace WearStoreWpf
{
    public class PriceWithDiscountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double price && parameter is int discount)
            {
                double discountedPrice = price - (price * discount / 100.0);
                return discountedPrice.ToString("C", CultureInfo.CurrentCulture);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
