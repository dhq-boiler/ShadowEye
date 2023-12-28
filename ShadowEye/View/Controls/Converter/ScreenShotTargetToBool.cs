

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Controls.Converter
{
    internal class ScreenShotTargetToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (ScreenShotDialogViewModel.ScreenShotTarget)value;
            var str = (string)parameter;
            switch (str)
            {
                case "VirtualScreen":
                    return val == ScreenShotDialogViewModel.ScreenShotTarget.VirtualScreen;
                case "Screen":
                    return val == ScreenShotDialogViewModel.ScreenShotTarget.Screen;
                case "Desktop":
                    return val == ScreenShotDialogViewModel.ScreenShotTarget.Desktop;
                case "Window":
                    return val = ScreenShotDialogViewModel.ScreenShotTarget.Window;
                default:
                    Debug.Assert(false, string.Format("value is {{{0}}}, parameter is {{{1}}}", value, parameter));
                    break;
            }
            throw new ArgumentException(string.Format("value is {{{0}}}, parameter is {{{1}}}", value, parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = (bool)value;
            if (!b) return ScreenShotDialogViewModel.ScreenShotTarget.Unknown;
            var str = (string)parameter;
            switch (str)
            {
                case "VirtualScreen" when b:
                    return ScreenShotDialogViewModel.ScreenShotTarget.VirtualScreen;
                case "Screen" when b:
                    return ScreenShotDialogViewModel.ScreenShotTarget.Screen;
                case "Desktop" when b:
                    return ScreenShotDialogViewModel.ScreenShotTarget.Desktop;
                case "Window" when b:
                    return ScreenShotDialogViewModel.ScreenShotTarget.Window;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
