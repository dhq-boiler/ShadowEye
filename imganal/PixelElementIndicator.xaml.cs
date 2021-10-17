

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

namespace imganal
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PixelElementIndicator : UserControl
    {
        public enum ElementType
        {
            B,
            G,
            R,
            H,
            S,
            V,
            ManhattanDistance,
            EuclideanDistance,
            OTHER
        }

        public class ElementInformationEntry
        {
            public ElementType ElementType { get; set; }
            public string LabelText { get; set; }
            public int MaxValue { get; set; }
            public Brush TypeLabelForeground { get; set; }
            public Brush ValueIndicatorBarFill { get; set; }
            public bool ValueIndicatorEnables { get; set; }
        }

        private List<ElementInformationEntry> _lineDefinition;

        public static readonly DependencyProperty Value1stProperty = DependencyProperty.Register("Value1st", typeof(int), typeof(PixelElementIndicator), new UIPropertyMetadata((s, e) => (s as PixelElementIndicator).UpdateIndicator()));
        public static readonly DependencyProperty Value2ndProperty = DependencyProperty.Register("Value2nd", typeof(int), typeof(PixelElementIndicator), new UIPropertyMetadata((s, e) => (s as PixelElementIndicator).UpdateIndicator()));
        public static readonly DependencyProperty Value3rdProperty = DependencyProperty.Register("Value3rd", typeof(int), typeof(PixelElementIndicator), new UIPropertyMetadata((s, e) => (s as PixelElementIndicator).UpdateIndicator()));
        public static readonly DependencyProperty Value4thProperty = DependencyProperty.Register("Value4th", typeof(int), typeof(PixelElementIndicator), new UIPropertyMetadata((s, e) => (s as PixelElementIndicator).UpdateIndicator()));
        public static readonly DependencyProperty Value5thProperty = DependencyProperty.Register("Value5th", typeof(int), typeof(PixelElementIndicator), new UIPropertyMetadata((s, e) => (s as PixelElementIndicator).UpdateIndicator()));
        public static readonly DependencyProperty Value6thProperty = DependencyProperty.Register("Value6th", typeof(int), typeof(PixelElementIndicator), new UIPropertyMetadata((s, e) => (s as PixelElementIndicator).UpdateIndicator()));

        public int Value1st
        {
            get { return (int)GetValue(Value1stProperty); }
            set { SetValue(Value1stProperty, value); }
        }

        public int Value2nd
        {
            get { return (int)GetValue(Value2ndProperty); }
            set { SetValue(Value2ndProperty, value); }
        }

        public int Value3rd
        {
            get { return (int)GetValue(Value3rdProperty); }
            set { SetValue(Value3rdProperty, value); }
        }

        public int Value4th
        {
            get { return (int)GetValue(Value4thProperty); }
            set { SetValue(Value4thProperty, value); }
        }

        public int Value5th
        {
            get { return (int)GetValue(Value5thProperty); }
            set { SetValue(Value5thProperty, value); }
        }

        public int Value6th
        {
            get { return (int)GetValue(Value6thProperty); }
            set { SetValue(Value6thProperty, value); }
        }

        public object[] Values { get; set; }
        public object Header
        {
            get { return HeaderText.Content; }
            set { HeaderText.Content = value; }
        }

        public PixelElementIndicator()
        {
            InitializeComponent();

            _lineDefinition = new List<ElementInformationEntry>();
        }

        public void AddLine(ElementInformationEntry elementInfo)
        {
            _lineDefinition.Add(elementInfo);
        }

        public void UpdateIndicator()
        {
            int value = 0;
            if (Values == null)
            {
                foreach (var row in stackpanel.Children)
                {
                    StackPanel panel = (row as StackPanel).Children[0] as StackPanel;

                    (panel.Children[1] as Label).Content = Level(value);

                    if (_lineDefinition[value].ValueIndicatorEnables)
                    {
                        ((row as StackPanel).Children[1] as Rectangle).Width = Level(value) / 255d * ActualWidth;
                    }

                    ++value;
                }
            }
            else
            {
                foreach (var row in stackpanel.Children)
                {
                    StackPanel panel = (row as StackPanel).Children[0] as StackPanel;

                    (panel.Children[1] as Label).Content = Values[value];

                    if (_lineDefinition[value].ValueIndicatorEnables)
                    {
                        ((row as StackPanel).Children[1] as Rectangle).Width = int.Parse(Values[value].ToString()) / 255d * ActualWidth;
                    }

                    ++value;
                }
            }
        }

        private int Level(int index)
        {
            switch (index)
            {
                case 0:
                    return Value1st;
                case 1:
                    return Value2nd;
                case 2:
                    return Value3rd;
                case 3:
                    return Value4th;
                case 4:
                    return Value5th;
                case 5:
                    return Value6th;
                default:
                    throw new NotSupportedException();
            }
        }

        public void UpdateFrame()
        {
            stackpanel.Children.Clear();

            foreach (var row in _lineDefinition)
            {
                var stackpanel_new = CreateStackPanel(row);
                stackpanel.Children.Add(stackpanel_new);
            }
        }

        private static StackPanel CreateStackPanel(ElementInformationEntry row)
        {
            StackPanel stackpanel_v = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 0, 0, 4)
            };

            {
                StackPanel stackpanel_h = new StackPanel() { Orientation = Orientation.Horizontal };
                Label typeLabel = new Label()
                {
                    Foreground = row.TypeLabelForeground,
                    Height = 25,
                    Content = row.LabelText != null ? row.LabelText : row.ElementType.ToString()
                };
                Label valueLabel = new Label()
                {
                    Foreground = row.TypeLabelForeground,
                    Height = 25,
                    HorizontalAlignment = HorizontalAlignment.Right
                };

                stackpanel_h.Children.Add(typeLabel);
                stackpanel_h.Children.Add(valueLabel);

                stackpanel_v.Children.Add(stackpanel_h);
            }
            Rectangle valueIndicateBar = new Rectangle()
            {
                Fill = row.ValueIndicatorBarFill,
                Width = 0,
                Height = 1,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            stackpanel_v.Children.Add(valueIndicateBar);

            return stackpanel_v;
        }

        public void RemoveLine(ElementInformationEntry elementInfo)
        {
            _lineDefinition.Remove(elementInfo);
        }
    }
}
