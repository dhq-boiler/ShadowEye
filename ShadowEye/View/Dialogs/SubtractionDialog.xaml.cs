

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
    /// Interaction logic for SubtractionDialog.xaml
    /// </summary>
    public partial class SubtractionDialog : Window
    {
        public SubtractionDialog()
        {
            InitializeComponent();
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            SubtractionDialogVM.AddComputingTab();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ComboBox_RightHand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubtractionDialogVM.SelectedRightHand != null)
            {
                SubtractionDialogVM.SelectedRightHand.HowToUpdate.RequestAccomplished();
            }

            SubtractionDialogVM.SelectedRightHand = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (SubtractionDialogVM.SelectedRightHand != null)
            {
                SubtractionDialogVM.SelectedRightHand.HowToUpdate.Request();
            }
        }

        private void ComboBox_LeftHand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubtractionDialogVM.SelectedLeftHand != null)
            {
                SubtractionDialogVM.SelectedLeftHand.HowToUpdate.RequestAccomplished();
            }

            SubtractionDialogVM.SelectedLeftHand = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (SubtractionDialogVM.SelectedLeftHand != null)
            {
                SubtractionDialogVM.SelectedLeftHand.HowToUpdate.Request();
            }
        }

        private SubtractionDialogViewModel SubtractionDialogVM
        {
            get { return this.DataContext as SubtractionDialogViewModel; }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (SubtractionDialogVM.SelectedLeftHand != null)
            {
                SubtractionDialogVM.SelectedLeftHand.HowToUpdate.Request();
            }
            if (SubtractionDialogVM.SelectedRightHand != null)
            {
                SubtractionDialogVM.SelectedRightHand.HowToUpdate.Request();
            }
        }
    }
}
