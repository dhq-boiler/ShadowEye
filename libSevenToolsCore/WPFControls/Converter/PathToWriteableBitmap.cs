// Copyright © 2015 dhq_boiler.

using System;
using System.Windows.Data;
using libSevenToolsCore.WPFControls.Imaging;

namespace libSevenToolsCore.WPFControls.Converter
{
    public class PathToWriteableBitmap : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = value as string;
            return Decorder.LoadBitmap(path);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
