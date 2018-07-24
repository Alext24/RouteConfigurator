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
            string param = (string)parameter;

            if(param != null && param.Equals("approve"))  //Approve check box
            {
                if(state == 3)  //Approve checked
                {
                    isChecked = true;
                }
            }
            else if(param != null && param.Equals("decline")) //Decline check box
            {
                if(state == 4) //Decline checked
                {
                    isChecked = true;
                }
            }
            return isChecked;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            int state = 0;  //Waiting state
            string param = (string)parameter;

            if (isChecked)
            {
                if (param != null && param.Equals("approve"))
                {
                    state = 3;  //Approve checked state
                }
                else if (param != null && param.Equals("decline"))
                {
                    state = 4;  //Decline checked state
                }
            }
            return state;
        }
    }
}
