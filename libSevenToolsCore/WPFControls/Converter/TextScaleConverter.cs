// Copyright © 2015 dhq_boiler.

using System;
using System.Windows.Data;

namespace libSevenToolsCore.WPFControls.Converter
{
    internal class TextScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var d = (double)value;
            var x = d * 100 % 1;
            if (x == 0)
                return d.ToString("P0");
            else
                return d.ToString("P2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = (string)value;
            s = s.Replace("%", "");
            return double.Parse(s) / 100;
        }
    }
}
