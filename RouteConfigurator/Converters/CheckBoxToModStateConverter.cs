using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator
{
    class CheckBoxToModStateConverter : BaseValueConverter<CheckBoxToModStateConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = false;
            int state = (int)value;

            if(state == 3)  //Checked state
            {
                isChecked = true;
            }
            return isChecked;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            int state = 0;  //Waiting state

            if (isChecked)
            {
                state = 3;  //Checked state
            }
            return state;
        }
    }
}
