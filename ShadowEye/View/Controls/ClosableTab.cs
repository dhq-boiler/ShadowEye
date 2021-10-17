

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ShadowEye.View.Controls
{
    internal class CloseableTab : TabItem
    {
        private CloseableHeader _closableTabHeader;
        public CloseableTab()
        {
            _closableTabHeader = new CloseableHeader();
            this.Header = _closableTabHeader;

            _closableTabHeader.Button_close.MouseEnter += Button_close_MouseEnter;
            _closableTabHeader.Button_close.MouseLeave += Button_close_MouseLeave;
            _closableTabHeader.Button_close.Click += Button_close_Click;
            _closableTabHeader.Label_TabTitle.SizeChanged += Label_TabTitle_SizeChanged;
        }

        private void Label_TabTitle_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ((CloseableHeader)this.Header).Button_close.Margin = new Thickness(((CloseableHeader)this.Header).Label_TabTitle.ActualWidth + 5, 3, 4, 0);
        }

        private void Button_close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((TabControl)this.Parent).Items.Remove(this);
        }

        private void Button_close_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).Button_close.Foreground = Brushes.Black;
        }

        private void Button_close_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).Button_close.Foreground = Brushes.Red;
        }

        public string Title
        {
            set
            {
                ((CloseableHeader)this.Header).Label_TabTitle.Content = value;
            }
        }

        protected override void OnSelected(System.Windows.RoutedEventArgs e)
        {
            base.OnSelected(e);
            ((CloseableHeader)this.Header).Button_close.Visibility = System.Windows.Visibility.Visible;
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((CloseableHeader)this.Header).Button_close.Visibility = System.Windows.Visibility.Visible;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.IsSelected)
            {
                ((CloseableHeader)this.Header).Button_close.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}
