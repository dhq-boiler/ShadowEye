// Copyright © 2015 dhq_boiler.

using System;
using System.Windows.Data;

namespace libSevenToolsCore.WPFControls.Converter
{
    internal class DoubleHalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d = (double)value;
            return d / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
