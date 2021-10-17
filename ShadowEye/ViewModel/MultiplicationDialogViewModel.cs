

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;

namespace ShadowEye.ViewModel
{
    public class MultiplicationDialogViewModel : CommandSink
    {
        private MainWorkbenchViewModel _ImageContainerVM;
        private AnalyzingSource _SelectedLeftHand;
        private AnalyzingSource _SelectedRightHand;
        private double _ScaleFactor = 1.0;
        private EColorMode _ColorMode;
        private static int s_createCount;

        public MultiplicationDialogViewModel(MainWorkbenchViewModel imageContainerVM)
        {
            _ImageContainerVM = imageContainerVM;
            _ColorMode = EColorMode.BGR;
        }

        public EColorMode ColorMode
        {
            get { return _ColorMode; }
            set { SetProperty<EColorMode>(ref _ColorMode, value, "ColorMode"); }
        }

        public ComboBoxItem[] LeftHand
        {
            get
            {
                return _ImageContainerVM.Tabs.Select(a => new ComboBoxItem() { Content = a.Source }).ToArray();
            }
        }

        public AnalyzingSource SelectedLeftHand
        {
            get { return _SelectedLeftHand; }
            set { SetProperty<AnalyzingSource>(ref _SelectedLeftHand, value, "SelectedLeftHand"); }
        }

        public ComboBoxItem[] RightHand
        {
            get
            {
                return _ImageContainerVM.Tabs.Select(a => new ComboBoxItem() { Content = a.Source }).ToArray();
            }
        }

        public AnalyzingSource SelectedRightHand
        {
            get { return _SelectedRightHand; }
            set { SetProperty<AnalyzingSource>(ref _SelectedRightHand, value, "SelectedRightHand"); }
        }

        public double ScaleFactor
        {
            get { return _ScaleFactor; }
            set { SetProperty<double>(ref _ScaleFactor, value, "ScaleFactor"); }
        }

        internal void AddComputingTab()
        {
            try
            {
                var source = new MultipliedSource(string.Format("Multiplied-{0}", ++s_createCount),
                    SelectedLeftHand, SelectedRightHand,
                    ScaleFactor,
                    ColorMode);
                _ImageContainerVM.AddOrActive(source);
            }
            catch (OpenCvSharp.OpenCVException e)
            {
                MessageBox.Show(e.ToString(), e.Message, MessageBoxButton.OK);
            }
        }
    }
}
