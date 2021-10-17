

using OpenCvSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;

namespace ShadowEye.ViewModel
{
    public class ColorConversionDialogViewModel : CommandSink
    {
        private MainWorkbenchViewModel _ImageContainerVM;
        private AnalyzingSource _TargetImage;
        private ColorConversionCodes _SelectedConversionType;
        private static int s_createCount;

        public ColorConversionDialogViewModel(MainWorkbenchViewModel imageContainerVM)
        {
            _ImageContainerVM = imageContainerVM;
        }

        public ComboBoxItem[] Items
        {
            get
            {
                return _ImageContainerVM.Tabs.Select(a => new ComboBoxItem() { Content = a.Source }).ToArray();
            }
        }

        public ColorConversionCodes[] ConversionType
        {
            get { return Enum.GetValues(typeof(ColorConversionCodes)).OfType<ColorConversionCodes>().OrderBy(a => a.ToString()).ToArray(); }
        }

        public ColorConversionCodes SelectedConversionType
        {
            get { return _SelectedConversionType; }
            set { SetProperty<ColorConversionCodes>(ref _SelectedConversionType, value, "SelectedConversionType"); }
        }

        internal void AddComputingTab()
        {
            try
            {
                var source = new ColorConvertedSource(string.Format("ColorConverted-{0}", ++s_createCount),
                    TargetImage, SelectedConversionType);
                _ImageContainerVM.AddOrActive(source);
            }
            catch (OpenCVException e)
            {
                MessageBox.Show(e.ToString(), e.Message, MessageBoxButton.OK);
            }
        }

        public AnalyzingSource TargetImage
        {
            get { return _TargetImage; }
            set { SetProperty<AnalyzingSource>(ref _TargetImage, value, "TargetImage"); }
        }
    }
}
