

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShadowEye.ViewModel;

namespace ShadowEye
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel MainWindowVM { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            base.DataContext = MainWindowVM = new MainWindowViewModel(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
