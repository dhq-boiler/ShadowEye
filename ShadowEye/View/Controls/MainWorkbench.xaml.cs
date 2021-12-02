

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OpenCvSharp;
using ShadowEye.Model;
using ShadowEye.ViewModel;

namespace ShadowEye.View.Controls
{
    /// <summary>
    /// Interaction logic for MainWorkbench.xaml
    /// </summary>
    public partial class MainWorkbench : UserControl
    {
        public MainWorkbench()
        {
            InitializeComponent();
        }

        private void TabControl_Workbench_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                var vm = item as ImageViewModel;
                var src = vm.Source as AnalyzingSource;
                if (src != null)
                    src.HowToUpdate.Request();
            }

            foreach (var item in e.RemovedItems)
            {
                var vm = item as ImageViewModel;
                var src = vm.Source as AnalyzingSource;
                if (src != null)
                    src.HowToUpdate.RequestAccomplished();
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            var dc = (sender as Button).DataContext;
            if (dc != null && dc is ImageViewModel)
            {
                ImageViewModel ivm = (ImageViewModel)dc;
                ivm.CloseThisTab();
            }
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TabItem titem = sender as TabItem;
            titem.Focus();
            base.OnMouseRightButtonDown(e);
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            var vm = DataContext as MainWorkbenchViewModel;
            foreach (var path in paths)
            {
                try
                {
                    if (Path.GetExtension(path) == ".gif")
                    {
                        var source = new FilmSource(path);
                        using (VideoCapture capture = new VideoCapture(path))
                        {
                            while (capture.IsOpened())
                            {
                                Mat mat = new Mat();

                                if (capture.Read(mat))
                                {
                                    source.Frames.Add(new Tuple<Mat, TimeSpan>(mat, TimeSpan.FromSeconds(1d / capture.Fps)));
                                }
                                else
                                {
                                    break;
                                }
                            }
                            source.ChannelType = libimgeng.ChannelType.BGR24;
                        }
                        vm.AddOrActive(source);
                    }
                    else if (Path.GetExtension(path) == ".mp4")
                    {
                        var source = new FilmSource(path);
                        using (VideoCapture capture = new VideoCapture(path))
                        {
                            while (capture.IsOpened())
                            {
                                Mat mat = new Mat();

                                if (capture.Read(mat))
                                {
                                    source.Frames.Add(new Tuple<Mat, TimeSpan>(mat, TimeSpan.FromSeconds(1d / capture.Fps)));
                                }
                                else
                                {
                                    break;
                                }
                            }
                            source.ChannelType = libimgeng.ChannelType.BGR24;
                        }
                        vm.AddOrActive(source);
                    }
                    else
                    {
                        vm.AddOrActive(new FileSource(path));
                    }
                }
                catch (ArgumentException)
                {
                    MessageBox.Show(string.Format("{0}\u306E\u8AAD\u307F\u8FBC\u307F\u306B\u5931\u6557\u3057\u307E\u3057\u305F\uFF0E\u30D5\u30A1\u30A4\u30EB\u304C\u58CA\u308C\u3066\u3044\u306A\u3044\u304B\uFF0C\u307E\u305F\u306F\u30AA\u30D5\u30E9\u30A4\u30F3\u3067\u5229\u7528\u3067\u304D\u308B\u304B\u78BA\u8A8D\u3057\u3066\u304F\u3060\u3055\u3044\uFF0E", path), "\u8AAD\u307F\u8FBC\u307F\u30A8\u30E9\u30FC", MessageBoxButton.OK);
                }
            }
        }

        private void ImageViewport_PixelPointed(object sender, libSevenTools.WPFControls.Imaging.ImageViewport.PixelPointedEventArgs e)
        {
            var levelIndicatorVM = (DataContext as MainWorkbenchViewModel).MainWindowVM.SubWorkbenchVM.LevelIndicatorVM;
            levelIndicatorVM.Value1st = e.R;
            levelIndicatorVM.Value2nd = e.G;
            levelIndicatorVM.Value3rd = e.B;
            levelIndicatorVM.Value4th = Math.Max(e.R, Math.Max(e.G, e.B));
        }
    }
}
