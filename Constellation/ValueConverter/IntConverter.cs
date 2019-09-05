using System;
using System.Windows.Data;

namespace Constellation.ValueConverter
{
    public class IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strVal = value?.ToString();

            if (string.IsNullOrEmpty(strVal))
                return 0;

            else
                return int.Parse(strVal);
        }



        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.ToString();
        }
    }
}
