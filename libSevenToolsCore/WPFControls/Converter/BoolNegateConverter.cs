// Copyright © 2015 dhq_boiler.

using System;
using System.Windows.Data;

namespace libSevenToolsCore.WPFControls.Converter
{
    internal class BoolNegateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = (bool)value;
            return !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
