using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator
{
    class DecimalTimeToStringConverter : BaseValueConverter<DecimalTimeToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "0";

            decimal hours = Math.Truncate((decimal)value);
            decimal minutes = Math.Round(((decimal)value - hours) * 60);

            string averageTimeText = string.Format("{0}:{1:00}", hours, minutes);
            return averageTimeText;

            //throw new NotImplementedException();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
