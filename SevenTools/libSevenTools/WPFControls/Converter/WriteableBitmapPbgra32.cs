// Copyright © 2015 dhq_boiler.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using libSevenTools.WPFControls.Imaging;

namespace libSevenTools.WPFControls.Converter
{
    public class WriteableBitmapPbgra32 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Decorder.ConvertToPbgra32(value as WriteableBitmap);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
