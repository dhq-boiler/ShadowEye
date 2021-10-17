

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
    /// Interaction logic for MultiplicationDialog.xaml
    /// </summary>
    public partial class MultiplicationDialog : Window
    {
        private MultiplicationDialogViewModel MultiplicationDialogVM
        {
            get { return this.DataContext as MultiplicationDialogViewModel; }
        }

        public MultiplicationDialog()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DialogResult == true)
            {
                if (MultiplicationDialogVM.SelectedLeftHand != null)
                {
                    MultiplicationDialogVM.SelectedLeftHand.HowToUpdate.Request();
                }
                if (MultiplicationDialogVM.SelectedRightHand != null)
                {
                    MultiplicationDialogVM.SelectedRightHand.HowToUpdate.Request();
                }
            }
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            MultiplicationDialogVM.AddComputingTab();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ComboBox_LeftHand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MultiplicationDialogVM.SelectedLeftHand != null)
            {
                MultiplicationDialogVM.SelectedLeftHand.HowToUpdate.RequestAccomplished();
            }

            MultiplicationDialogVM.SelectedLeftHand = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (MultiplicationDialogVM.SelectedLeftHand != null)
            {
                MultiplicationDialogVM.SelectedLeftHand.HowToUpdate.Request();
            }
        }

        private void ComboBox_RightHand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MultiplicationDialogVM.SelectedRightHand != null)
            {
                MultiplicationDialogVM.SelectedRightHand.HowToUpdate.RequestAccomplished();
            }

            MultiplicationDialogVM.SelectedRightHand = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (MultiplicationDialogVM.SelectedRightHand != null)
            {
                MultiplicationDialogVM.SelectedRightHand.HowToUpdate.Request();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double failsafe = MultiplicationDialogVM.ScaleFactor;
            try
            {
                var text = (sender as TextBox).Text;
                double parsed = double.Parse(text);
                if (!parsed.ToString().Equals(text))
                    return;
                if (parsed == double.NaN)
                {
                    MultiplicationDialogVM.ScaleFactor = failsafe;
                }
                else
                {
                    MultiplicationDialogVM.ScaleFactor = parsed;
                }
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }
        }
    }
}
