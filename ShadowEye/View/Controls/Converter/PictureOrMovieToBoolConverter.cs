

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Controls.Converter
{
    internal class PictureOrMovieToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (ScreenShotDialogViewModel.PictureOrMovie)value;
            var str = (string)parameter;
            switch (str)
            {
                case "Picture":
                    return val == ScreenShotDialogViewModel.PictureOrMovie.Picture;
                case "Movie":
                    return val == ScreenShotDialogViewModel.PictureOrMovie.Movie;
                case "Film":
                    return val == ScreenShotDialogViewModel.PictureOrMovie.Film;
                default:
                    Debug.Assert(false, string.Format("value is {{{0}}}, parameter is {{{1}}}", value, parameter));
                    break;
            }
            throw new ArgumentException(string.Format("value is {{{0}}}, parameter is {{{1}}}", value, parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = (string)parameter;
            switch (str)
            {
                case "Picture":
                    return ScreenShotDialogViewModel.PictureOrMovie.Picture;
                case "Movie":
                    return ScreenShotDialogViewModel.PictureOrMovie.Movie;
                case "Film":
                    return ScreenShotDialogViewModel.PictureOrMovie.Film;
                default:
                    Debug.Assert(false, string.Format("value is {{{0}}}, parameter is {{{1}}}", value, parameter));
                    break;
            }
            throw new ArgumentException(string.Format("value is {{{0}}}, parameter is {{{1}}}", value, parameter));
        }
    }
}
