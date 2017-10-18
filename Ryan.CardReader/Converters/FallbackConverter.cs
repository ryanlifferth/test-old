using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ryan.CardReader.Converters
{
    public class FallbackConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var month = $"{values[0]}";
            var year = $"{values[1]}";

            if (month == "" && year == "")
            {
                return "";
            }
            else
            {
                return $"{month}/{year}";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { Binding.DoNothing, false };
        }
    }
}
