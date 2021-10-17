// Copyright © 2015 dhq_boiler.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using libSevenTools.WPFControls.Imaging;

namespace ImageViewportTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageViewport.Uniform();
        }

        private void ImageViewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowInfo();
        }

        private void ImageViewport_ViewportRendered(object sender, RoutedEventArgs e)
        {
            ShowInfo();
        }

        private void ShowInfo()
        {
            Label_SourceSize.Content = string.Format("SourceSize : {0}", ImageViewport.SourceSize);
            Label_RenderingAreaSize.Content = string.Format("RenderingAreaSize : {0}", ImageViewport.RenderingAreaSize);
            Label_SourceOffsetPoint.Content = string.Format("SourceOffsetPoint : {0}", ImageViewport.SourceOffsetPoint);
            Label_ScaleFactor.Content = string.Format("ScaleFactor : {0}", ImageViewport.ScaleFactor);
            try
            {
                Label_RenderedRenderingAreaRect.Content = string.Format("RenderedRenderingAreaRect : {0}", ImageViewport.RenderedRenderingAreaRect);
            }
            catch (NotIncludeException)
            {
                Label_RenderedRenderingAreaRect.Content = "N/A";
            }
            try
            {
                Label_RenderedSourceRect.Content = string.Format("RenderedSourceRect : {0}", ImageViewport.RenderedSourceRect);
            }
            catch (NotIncludeException)
            {
                Label_RenderedSourceRect.Content = "N/A";
            }
            Label_ScaledSourceSize.Content = string.Format("ScaledSourceSize : {0}", ImageViewport.ScaledSourceSize);
            Label_ViewportRect.Content = string.Format("ViewportRect : {0}", ImageViewport.ViewportRect);
            Label_ViewportCenterDefault.Content = string.Format("ViewportCenterDefault : {0}", ImageViewport.ViewportCenterDefault);
            Label_ViewportCenterCurrent.Content = string.Format("ViewportCenterCurrent : {0}", ImageViewport.ViewportCenterCurrent);
            Label_ViewportLeftTopDefault.Content = string.Format("ViewportLeftTopDefault : {0}", ImageViewport.ViewportLeftTopDefault);
            Label_ViewportLeftTopCurrent.Content = string.Format("ViewportLeftTopCurrent : {0}", ImageViewport.ViewportLeftTopCurrent);
            Label_ViewportBottomRightDefault.Content = string.Format("ViewportBottomRightDefault : {0}", ImageViewport.ViewportBottomRightDefault);
            Label_ViewportBottomRightCurrent.Content = string.Format("ViewportBottomRightCurrent : {0}", ImageViewport.ViewportBottomRightCurrent);
        }

        private void ImageViewport_PixelPointed(object sender, ImageViewport.PixelPointedEventArgs e)
        {
            Trace.WriteLine(string.Format("(X, Y, B, G, R, A) = ({0}, {1}, {2}, {3}, {4}, {5})", e.X, e.Y, e.B, e.G, e.R, e.A));
            Label_Pointed.Content = string.Format("MousePointed : ({0}, {1})", e.X, e.Y);
        }
    }
}
