// Copyright © 2015 dhq_boiler.

using System;
using System.Windows;
using System.Windows.Data;

namespace libSevenToolsCore.WPFControls.Converter
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? IsVisible = (bool?)value;
            bool inverse = (bool)parameter;
            if (IsVisible.HasValue)
            {
                if (IsVisible.Value ^ inverse)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
