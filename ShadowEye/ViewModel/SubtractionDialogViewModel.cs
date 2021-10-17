

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
    public class SubtractionDialogViewModel : CommandSink
    {
        private MainWorkbenchViewModel _ImageContainerVM;
        private bool _IsAbsolute;
        private AnalyzingSource _SelectedLeftHand;
        private AnalyzingSource _SelectedRightHand;
        private EColorMode _ColorMode;
        private static int s_createdCount;

        public SubtractionDialogViewModel(MainWorkbenchViewModel imageContainerVM)
        {
            ImageContainerVM = imageContainerVM;
            ColorMode = EColorMode.BGR;
        }

        public MainWorkbenchViewModel ImageContainerVM
        {
            get { return _ImageContainerVM; }
            set { SetProperty<MainWorkbenchViewModel>(ref _ImageContainerVM, value, "ImageContainerVM"); }
        }

        public EColorMode ColorMode
        {
            get { return _ColorMode; }
            set { SetProperty<EColorMode>(ref _ColorMode, value, "ColorMode"); }
        }

        public bool IsAbsolute
        {
            get { return _IsAbsolute; }
            set { SetProperty<bool>(ref _IsAbsolute, value, "IsAbsolute"); }
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

        public void AddComputingTab()
        {
            try
            {
                var source = new SubtractedSource(string.Format("Subtract-{0}", ++s_createdCount),
                    SelectedLeftHand, SelectedRightHand,
                    IsAbsolute ? ComputingMethod.Subtract_Absolute : ComputingMethod.Subtract,
                    ColorMode);
                (App.Current.MainWindow as MainWindow).MainWindowVM.ImageContainerVM.AddOrActive(source);
            }
            catch (OpenCvSharp.OpenCVException e)
            {
                MessageBox.Show(e.ToString(), e.Message, MessageBoxButton.OK);
            }
        }
    }
}
