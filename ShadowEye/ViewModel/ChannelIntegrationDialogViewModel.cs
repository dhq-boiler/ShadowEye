

using OpenCvSharp;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;

namespace ShadowEye.ViewModel
{
    public class ChannelIntegrationDialogViewModel : CommandSink
    {
        private MainWorkbenchViewModel _ImageContainerVM;
        private bool _IsNormalize;
        private AnalyzingSource _SelectedItem;
        private static int s_createCount;
        private MatType _SelectedCalcuratingMatType;

        public ChannelIntegrationDialogViewModel(MainWorkbenchViewModel imageContainerVM)
        {
            ImageContainerVM = imageContainerVM;
        }

        public MainWorkbenchViewModel ImageContainerVM
        {
            get { return _ImageContainerVM; }
            set { SetProperty<MainWorkbenchViewModel>(ref _ImageContainerVM, value, "ImageContainerVM"); }
        }

        public bool IsNormalize
        {
            get { return _IsNormalize; }
            set { SetProperty<bool>(ref _IsNormalize, value, "IsNormalize"); }
        }

        public ComboBoxItem[] Items
        {
            get
            {
                return _ImageContainerVM.Tabs.Select(a => new ComboBoxItem() { Content = a.Source }).ToArray();
            }
        }

        public AnalyzingSource SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty<AnalyzingSource>(ref _SelectedItem, value, "SelectedItem", "SelectedItemChannel"); }
        }

        public int SelectedItemChannel
        {
            get { return SelectedItem != null ? SelectedItem.Channels : -1; }
        }

        public MatType[] MatTypes
        {
            get
            {
                return new MatType[]
                {
                    MatType.CV_8SC1,
                    MatType.CV_8UC1,
                    MatType.CV_16SC1,
                    MatType.CV_16UC1,
                    MatType.CV_32SC1,
                    MatType.CV_32FC1,
                    MatType.CV_64FC1,
                };
            }
        }

        public MatType SelectedCalcuratingMatType
        {
            get { return _SelectedCalcuratingMatType; }
            set { SetProperty<MatType>(ref _SelectedCalcuratingMatType, value, "SelectedCalcuratingMatType"); }
        }

        public void AddComputingTab()
        {
            try
            {
                var source = new ChannelIntegratedSource(
                    string.Format("IntegrateChannel-{0}", ++s_createCount),
                    SelectedItem,
                    IsNormalize ? ComputingMethod.IntegrateChannel_Normalize : ComputingMethod.IntegrateChannel,
                    SelectedCalcuratingMatType);
                (App.Current.MainWindow as MainWindow).MainWindowVM.ImageContainerVM.AddOrActive(source);
            }
            catch (OpenCvSharp.OpenCVException e)
            {
                MessageBox.Show(e.ToString(), e.Message, MessageBoxButton.OK);
            }
        }
    }
}
