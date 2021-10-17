

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
using imganal;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Controls
{
    /// <summary>
    /// Interaction logic for SubWorkbench.xaml
    /// </summary>
    public partial class SubWorkbench : UserControl
    {
        public SubWorkbench()
        {
            InitializeComponent();

            LevelIndicator.Header = new Label() { Content = "Level", Foreground = new SolidColorBrush() { Color = Colors.White } };

            LevelIndicator.AddLine(new PixelElementIndicator.ElementInformationEntry()
            {
                ElementType = PixelElementIndicator.ElementType.R,
                LabelText = "R",
                TypeLabelForeground = new SolidColorBrush(Colors.White),
                MaxValue = 255,
                ValueIndicatorBarFill = new SolidColorBrush(Colors.Red),
                ValueIndicatorEnables = true
            });

            LevelIndicator.AddLine(new PixelElementIndicator.ElementInformationEntry()
            {
                ElementType = PixelElementIndicator.ElementType.G,
                LabelText = "G",
                TypeLabelForeground = new SolidColorBrush(Colors.White),
                MaxValue = 255,
                ValueIndicatorBarFill = new SolidColorBrush(Colors.Green),
                ValueIndicatorEnables = true
            });

            LevelIndicator.AddLine(new PixelElementIndicator.ElementInformationEntry()
            {
                ElementType = PixelElementIndicator.ElementType.B,
                LabelText = "B",
                TypeLabelForeground = new SolidColorBrush(Colors.White),
                MaxValue = 255,
                ValueIndicatorBarFill = new SolidColorBrush(Colors.Blue),
                ValueIndicatorEnables = true
            });

            LevelIndicator.AddLine(new PixelElementIndicator.ElementInformationEntry()
            {
                ElementType = PixelElementIndicator.ElementType.V,
                LabelText = "V",
                TypeLabelForeground = new SolidColorBrush(Colors.White),
                MaxValue = 255,
                ValueIndicatorBarFill = new SolidColorBrush(Colors.White),
                ValueIndicatorEnables = true
            });

            LevelIndicator.UpdateFrame();
        }
    }
}
