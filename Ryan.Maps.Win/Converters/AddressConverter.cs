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
    public class AddressConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var addressLine1 = values[0] == null || values[0].ToString() == "" ? "" : values[0];
            var addressLine2 = values[1] == null || values[1].ToString() == "" ? "" : ", " + values[1];

            /*var server = $"{values[0]}0";
            var instance = $"{values[1]}";

            if (instance == "")
                return server;

            if (server == "")
                return "";

            return $"{server}\\{instance}";*/

            return $"{addressLine1}{addressLine2}";
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
