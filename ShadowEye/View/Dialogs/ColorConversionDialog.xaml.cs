

using OpenCvSharp;
using System;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ColorConversionDialog.xaml
    /// </summary>
    public partial class ColorConversionDialog : System.Windows.Window
    {
        private ColorConversionDialogViewModel ColorConversionDialogVM
        {
            get { return this.DataContext as ColorConversionDialogViewModel; }
        }

        public ColorConversionDialog()
        {
            InitializeComponent();
        }

        private void ComboBox_Target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorConversionDialogVM.TargetImage != null)
            {
                ColorConversionDialogVM.TargetImage.HowToUpdate.RequestAccomplished();
            }

            ColorConversionDialogVM.TargetImage = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (ColorConversionDialogVM.TargetImage != null)
            {
                ColorConversionDialogVM.TargetImage.HowToUpdate.Request();
            }
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            ColorConversionDialogVM.AddComputingTab();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DialogResult == true)
            {
                if (ColorConversionDialogVM.TargetImage != null)
                {
                    ColorConversionDialogVM.TargetImage.HowToUpdate.Request();
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ColorConversionDialogVM.SelectedConversionType = (ColorConversionCodes)(sender as ComboBox).SelectedItem;
        }
    }
}
