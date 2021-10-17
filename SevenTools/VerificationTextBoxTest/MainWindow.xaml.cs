using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VerificationTextBoxTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Text { get; set; }
        public bool? TextIsValid { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            VerificationTextBox.TextVerifier = new Func<string, bool?>((s) => System.IO.Directory.Exists(s));
            VerificationTextBox_2.TextVerifier = new Func<string, bool?>((s) => System.IO.Directory.Exists(s));
            VerificationTextBox_3.TextVerifier = new Func<string, bool?>((s) => System.IO.Directory.Exists(s));
        }

        private void VerificationTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(Text, "Text");
        }

        private void VerificationTextBox_TextIsValidChanged(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(TextIsValid, "TextIsValid");
        }
    }
}
