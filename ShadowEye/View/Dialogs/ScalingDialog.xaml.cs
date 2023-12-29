

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
    /// Interaction logic for ScalingDialog.xaml
    /// </summary>
    public partial class ScalingDialog : Window
    {
        private ScalingDialogViewModel ScalingDialogVM
        {
            get { return this.DataContext as ScalingDialogViewModel; }
        }

        public ScalingDialog()
        {
            InitializeComponent();
        }

        private void ComboBox_Target_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScalingDialogVM.SelectedItem != null)
            {
                ScalingDialogVM.SelectedItem.HowToUpdate.RequestAccomplished();
            }

            ScalingDialogVM.SelectedItem = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as AnalyzingSource;

            if (ScalingDialogVM.SelectedItem != null)
            {
                ScalingDialogVM.SelectedItem.HowToUpdate.Request();
            }
        }

        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            ScalingDialogVM.AddComputingTab();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (ScalingDialogVM.SelectedItem != null)
            {
                ScalingDialogVM.SelectedItem.HowToUpdate.RequestAccomplished();
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ScalingDialogVM != null)
                ScalingDialogVM.SizeIsPercent = true;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            if (ScalingDialogVM != null)
                ScalingDialogVM.SizeIsPercent = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ScalingDialogVM != null)
            {
                double failsafeWidth = ScalingDialogVM.Width;
                double failsafeHeight = ScalingDialogVM.Height;
                try
                {
                    var text = (sender as TextBox).Text;
                    double parsed = double.Parse(text);
                    if (!parsed.ToString().Equals(text))
                        return;
                    if (parsed == double.NaN)
                    {
                        ScalingDialogVM.Width = failsafeWidth;
                    }
                    else
                    {
                        ScalingDialogVM.Width = parsed;
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

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (ScalingDialogVM != null)
            {
                double failsafeWidth = ScalingDialogVM.Width;
                double failsafeHeight = ScalingDialogVM.Height;
                try
                {
                    var text = (sender as TextBox).Text;
                    double parsed = double.Parse(text);
                    if (!parsed.ToString().Equals(text))
                        return;
                    if (parsed == double.NaN)
                    {
                        ScalingDialogVM.Height = failsafeHeight;
                    }
                    {
                        ScalingDialogVM.Height = parsed;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ScalingDialogVM != null)
            {
                var bitmap = ScalingDialogVM.SelectedItem.Bitmap;
                if (ScalingDialogVM.SizeIsPercent)
                {
                    ScalingDialogVM.Width = 100;
                    ScalingDialogVM.Height = 100;
                }
                else
                {
                    ScalingDialogVM.Width = bitmap.Value.Width;
                    ScalingDialogVM.Height = bitmap.Value.Height;
                }
            }
        }
    }
}
