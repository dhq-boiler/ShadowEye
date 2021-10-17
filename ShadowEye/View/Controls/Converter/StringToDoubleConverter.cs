

using System;
using System.Windows.Data;

namespace ShadowEye.View.Controls.Converter
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (s == null) return null;
            return double.Parse(s);
        }
    }
}
