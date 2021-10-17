

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using ShadowEye.Model;

namespace ShadowEye.View.Controls.Converter
{
    public class ScreenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var screens = value as Screen[];
            List<ScreenOption> ret = new List<ScreenOption>();
            foreach (var screen in screens)
            {
                ret.Add(new ScreenOption(screen));
            }
            return ret.ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
