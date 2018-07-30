using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator
{
    class IntStateToTextStateConverter : BaseValueConverter<IntStateToTextStateConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stateText = "";
            int state = (int)value;

            switch (state)
            {
                case (0):
                    {
                        stateText = "WAITING";
                        break;
                    }
                case (1):
                    {
                        stateText = "APPROVED";
                        break;
                    }
                case (2):
                    {
                        stateText = "DECLINED";
                        break;
                    }
                case (3):   //Actually in currently checked to approve state
                    {
                        stateText = "WAITING";
                        break;
                    }
                case (4):   //Actually in currently checked to deny state
                    {
                        stateText = "WAITING";
                        break;
                    }
                default:
                    break;
            }

            return stateText;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stateText = (string)value;
            int state = 0;  //Waiting state

            switch (stateText)
            {
                case ("WAITING"):
                    {
                        state = 0; 
                        break;
                    }
                case ("APPROVED"):
                    {
                        state = 1;
                        break;
                    }
                case ("DECLINED"):
                    {
                        state = 2;
                        break;
                    }
                default:
                    break;
            }

            return state;
        }
    }
}
