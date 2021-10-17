

using OpenCvSharp;
using System;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ChannelIntegrationDialog.xaml
    /// </summary>
    public partial class ChannelIntegrationDialog : System.Windows.Window
    {
        private ChannelIntegrationDialogViewModel ChannelIntegrationDialogVM
        {
            get { return this.DataContext as ChannelIntegrationDialogViewModel; }
        }

        public ChannelIntegrationDialog()
        {
            InitializeComponent();
        }

        private void ComboBox_Target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChannelIntegrationDialogVM.SelectedItem != null)
            {
                ChannelIntegrationDialogVM.SelectedItem.HowToUpdate.RequestAccomplished();
            }

            ChannelIntegrationDialogVM.SelectedItem = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (ChannelIntegrationDialogVM.SelectedItem != null)
            {
                ChannelIntegrationDialogVM.SelectedItem.HowToUpdate.Request();
            }
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            ChannelIntegrationDialogVM.AddComputingTab();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (ChannelIntegrationDialogVM.SelectedItem != null)
            {
                ChannelIntegrationDialogVM.SelectedItem.HowToUpdate.RequestAccomplished();
            }
        }

        private void ComboBox_CalcuratingMatType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChannelIntegrationDialogVM.SelectedCalcuratingMatType = (MatType)(sender as ComboBox).SelectedItem;
        }
    }
}
