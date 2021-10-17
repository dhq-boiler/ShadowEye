

using OpenCvSharp;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Dialogs
{
    /// <summary>
    /// Interaction logic for EdgeExtractionDialog.xaml
    /// </summary>
    public partial class EdgeExtractionDialog : System.Windows.Window
    {
        private EdgeExtractionDialogViewModel eedVM { get { return DataContext as EdgeExtractionDialogViewModel; } }

        public EdgeExtractionDialog()
        {
            InitializeComponent();
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                eedVM.AddComputingTab();
                DialogResult = true;
            }
            catch (OpenCVException ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ComboBox_Target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (eedVM.TargetImage != null)
            {
                eedVM.TargetImage.HowToUpdate.RequestAccomplished();
            }

            eedVM.TargetImage = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (eedVM.TargetImage != null)
            {
                eedVM.TargetImage.HowToUpdate.Request();
            }
        }
    }
}
