using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Ryan.Maps.Win.Converters
{
    public class CityStateZipConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var city  = values[0] == null || values[0].ToString() == "" ? "" : values[0];
            var state = values[1] == null || values[1].ToString() == "" ? "" : ", " + values[1];
            var zip   = values[2] == null || values[2].ToString() == "" ? "" : " " + values[2];

            return $"{city}{state}{zip}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
