// Copyright © 2015 dhq_boiler.

using System;
using System.Windows.Data;
using System.Windows.Media;

namespace libSevenTools.WPFControls.Converter
{
    internal class BoolToBrushRedOrGreen : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? IsValid = (bool?)value;
            bool inverse = (bool)parameter;
            return new SolidColorBrush(IsValid.HasValue ? (IsValid.Value ^ inverse ? Colors.Lime : Colors.Red) : Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
