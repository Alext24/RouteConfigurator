﻿using System;
using System.Globalization;

namespace RouteConfigurator
{
    class DecimalTimeToStringConverter : BaseValueConverter<DecimalTimeToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            decimal hours = Math.Truncate((decimal)value);
            decimal minutes = Math.Round(((decimal)value - hours) * 60);

            string timeText = string.Format("{0}:{1:00}", hours, minutes);
            return timeText;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string timeText = value.ToString();
            decimal time = 0;

            // Colon format   12:15
            if (timeText.Contains(":"))
            {
                int colonIndex = timeText.IndexOf(':');
                int a = colonIndex + 1;
                int b = timeText.Length - 1 - colonIndex;

                string hoursText;
                string minutesText;

                decimal hours;
                decimal minutes;

                try
                {
                    if (colonIndex != 0)
                    {
                        hoursText = timeText.Substring(0, colonIndex);
                        hours = decimal.Parse(hoursText);
                    }
                    else
                    {
                        hours = 0;
                    }
                    minutesText = timeText.Substring(a, b);
                    minutes = decimal.Parse(minutesText);

                    time = hours + (minutes / 60);
                }catch (Exception)
                {
                    time = 0;
                }
            }
            // Decimal format   12.25 or 12
            else
            {
                try
                {
                    time = decimal.Parse(timeText);
                }
                catch (Exception)
                {
                    time = 0;
                }
            }

            return time;
        }
    }
}
