

using System;
using System.Globalization;
using System.Windows.Data;

namespace ShadowEye.View.Controls.Converter
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            if (s == null) return null;
            return int.Parse(s);
        }
    }
}
