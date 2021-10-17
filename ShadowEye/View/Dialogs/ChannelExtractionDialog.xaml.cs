

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ShadowEye.Model;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ChannelExtractionDialog.xaml
    /// </summary>
    public partial class ChannelExtractionDialog : Window
    {
        private ChannelExtractionDialogViewModel ChannelExtractionDialogVM
        {
            get { return this.DataContext as ChannelExtractionDialogViewModel; }
        }

        public ChannelExtractionDialog()
        {
            InitializeComponent();
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            ChannelExtractionDialogVM.AddComputingTab();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ComboBox_Target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChannelExtractionDialogVM.TargetImage != null)
            {
                ChannelExtractionDialogVM.TargetImage.HowToUpdate.RequestAccomplished();
            }

            ChannelExtractionDialogVM.TargetImage = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (ChannelExtractionDialogVM.TargetImage != null)
            {
                ChannelExtractionDialogVM.TargetImage.HowToUpdate.Request();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DialogResult == true)
            {
                if (ChannelExtractionDialogVM.TargetImage != null)
                {
                    ChannelExtractionDialogVM.TargetImage.HowToUpdate.Request();
                }
            }
        }
    }
}
