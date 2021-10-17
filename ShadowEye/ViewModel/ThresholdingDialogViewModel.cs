

using OpenCvSharp;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;

namespace ShadowEye.ViewModel
{
    public class ThresholdingDialogViewModel : CommandSink
    {
        private MainWorkbenchViewModel _MainWorkbenchVM;
        private AnalyzingSource _SelectedItem;
        private static int s_createCount;
        private ComputingMethod _SelectedMethod;
        private double _Threshold;
        private double _ThresholdingMaxValue;

        public ThresholdingDialogViewModel(MainWorkbenchViewModel imageContainerVM)
        {
            _MainWorkbenchVM = imageContainerVM;
        }

        public MainWorkbenchViewModel MainWorkbenchVM
        {
            get { return _MainWorkbenchVM; }
            set { SetProperty<MainWorkbenchViewModel>(ref _MainWorkbenchVM, value, "MainWorkbenchVM"); }
        }

        public ComboBoxItem[] Items
        {
            get
            {
                return _MainWorkbenchVM.Tabs.Select(a => new ComboBoxItem() { Content = a.Source }).ToArray();
            }
        }

        public AnalyzingSource SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty<AnalyzingSource>(ref _SelectedItem, value, "SelectedItem"); }
        }

        public ComputingMethod SelectedThresholdMethod
        {
            get { return _SelectedMethod; }
            set { SetProperty<ComputingMethod>(ref _SelectedMethod, value, "SelectedThresholdMethod"); }
        }

        public ComputingMethod[] ThresholdMethods
        {
            get
            {
                return new ComputingMethod[]
                {
                    ComputingMethod.Threshold_Binary,
                    ComputingMethod.Threshold_Binary_Inverse,
                    ComputingMethod.Threshold_ToZero,
                    ComputingMethod.Threshold_ToZero_Inverse,
                    ComputingMethod.Threshold_Trunc
                };
            }
        }

        public double Threshold
        {
            get { return _Threshold; }
            set { SetProperty<double>(ref _Threshold, value, "Threshold"); }
        }

        public double ThresholdingMaxValue
        {
            get { return _ThresholdingMaxValue; }
            set { SetProperty<double>(ref _ThresholdingMaxValue, value, "ThresholdingMaxValue"); }
        }

        public void AddComputingTab()
        {
            try
            {
                var source = new ThresholdedSource(
                    string.Format("Thresholding-{0}", ++s_createCount),
                    SelectedItem,
                    SelectedThresholdMethod,
                    Threshold,
                    ThresholdingMaxValue);
                _MainWorkbenchVM.AddOrActive(source);
            }
            catch (OpenCVException e)
            {
                MessageBox.Show(e.ToString(), e.Message, MessageBoxButton.OK);
            }
        }
    }
}
