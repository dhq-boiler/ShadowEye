

using System;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ThresholdingDialog.xaml
    /// </summary>
    public partial class ThresholdingDialog : System.Windows.Window
    {
        private ThresholdingDialogViewModel ThresholdingDialogVM
        {
            get { return this.DataContext as ThresholdingDialogViewModel; }
        }

        public ThresholdingDialog()
        {
            InitializeComponent();
        }

        private void ComboBox_Target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThresholdingDialogVM.SelectedItem != null)
            {
                ThresholdingDialogVM.SelectedItem.HowToUpdate.RequestAccomplished();
            }

            ThresholdingDialogVM.SelectedItem = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (ThresholdingDialogVM.SelectedItem != null)
            {
                ThresholdingDialogVM.SelectedItem.HowToUpdate.Request();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            ThresholdingDialogVM.AddComputingTab();
            DialogResult = true;
        }

        private void ComboBox_ThresholdMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThresholdingDialogVM.SelectedThresholdMethod = (ComputingMethod)(sender as ComboBox).SelectedItem;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (ThresholdingDialogVM.SelectedItem != null)
            {
                ThresholdingDialogVM.SelectedItem.HowToUpdate.RequestAccomplished();
            }
        }
    }
}
