using imganalCore;
using System.Windows.Controls;
using System.Windows.Media;

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
