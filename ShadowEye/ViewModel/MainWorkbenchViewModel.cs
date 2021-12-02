

using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using ShadowEye.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ShadowEye.ViewModel
{
    public class MainWorkbenchViewModel : CommandSink
    {
        private int _selectedIndex;
        private Dictionary<string, AnalyzingSource> _tab_collection;
        private ImageViewModel _selectedImageVM;
        public MainWindowViewModel MainWindowVM { get; private set; }

        public MainWorkbenchViewModel(MainWindowViewModel mainwindowVM)
        {
            _tab_collection = new Dictionary<string, AnalyzingSource>();
            Tabs = new ObservableCollection<ImageViewModel>();
            this.MainWindowVM = mainwindowVM;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            base.RegisterCommand(RoutedCommands.SaveAsCommand, (object p) => true, (object p) =>
            {
                this.SaveAsDialogOpen(p as ImageViewModel);
            });
            base.RegisterCommand(RoutedCommands.CloseThisTabCommand, (object p) => true, (object p) =>
            {
                this.CloseTab(p as ImageViewModel);
            });
            base.RegisterCommand(RoutedCommands.StoreDiscadedImageCommand, (object p) => true, (object p) =>
            {
                ImageViewModel ivm = p as ImageViewModel;
                AnalyzingSource asrc = ivm.Source as AnalyzingSource;
                this.MainWindowVM.ImageContainerVM.AddOrActive(new DiscadedSource(asrc));
            });
        }

        internal void SaveAsDialogOpen(AnalyzingSource source)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = source.Name;
            dialog.DefaultExt = "jpg";
            if (source is FilmSource)
            {
                dialog.Filter = GetExtensions(true, true);
            }
            else
            {
                dialog.Filter = GetExtensions(false, false);
            }
            if (dialog.ShowDialog() == true)
            {
                if (Path.GetExtension(dialog.FileName) == ".gif")
                {
                    GifBitmapEncoder encoder = new GifBitmapEncoder();
                    var filmSource = source as FilmSource;
                    var list = filmSource.Frames.Skip(filmSource.SelectionStart.Value + 1);
                    foreach (var mat in list.Take(filmSource.SelectionEnd.Value - filmSource.SelectionStart.Value).Select(x => x.Item1))
                    {
                        encoder.Frames.Add(BitmapFrame.Create(WriteableBitmapConverter.ToWriteableBitmap(mat)));
                    }
                    using (var stream = File.Create(dialog.FileName))
                    {
                        encoder.Save(stream);
                    }
                }
                else if (Path.GetExtension(dialog.FileName) == ".mp4")
                {
                    var filmSource = source as FilmSource;
                    using (VideoWriter videoWriter = new VideoWriter(dialog.FileName, FourCC.H264, filmSource.Frames.Count() / (filmSource.Frames.Sum(x => x.Item2.Milliseconds) / 1000d), new Size(source.Mat.Width, source.Mat.Height)))
                    {
                        foreach (var mat in (source as FilmSource).Frames.Select(x => x.Item1))
                        {
                            videoWriter.Write(mat);
                        }
                    }
                }
                else
                {
                    source.Mat.SaveImage(dialog.FileName);
                }
            }
        }

        private void SaveAsDialogOpen(ImageViewModel ivm)
        {
            if (ivm == null)
            {
                Trace.WriteLine("ivm is null");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = ivm.Source.Name;
            dialog.DefaultExt = "jpg";
            if (ivm.Source is FilmSource)
            {
                dialog.Filter = GetExtensions(true, true);
            }
            else
            {
                dialog.Filter = GetExtensions(false, false);
            }
            if (dialog.ShowDialog() == true)
            {
                if (Path.GetExtension(dialog.FileName) == ".gif")
                {
                    GifBitmapEncoder encoder = new GifBitmapEncoder();
                    var filmSource = ivm.Source as FilmSource;
                    var list = filmSource.Frames.Skip(filmSource.SelectionStart.Value + 1);
                    foreach (var mat in list.Take(filmSource.SelectionEnd.Value - filmSource.SelectionStart.Value).Select(x => x.Item1))
                    {
                        encoder.Frames.Add(BitmapFrame.Create(WriteableBitmapConverter.ToWriteableBitmap(mat)));
                    }
                    using (var stream = File.Create(dialog.FileName))
                    {
                        encoder.Save(stream);
                    }
                }
                else if (Path.GetExtension(dialog.FileName) == ".mp4")
                {
                    var source = ivm.Source as FilmSource;
                    using (VideoWriter videoWriter = new VideoWriter(dialog.FileName, FourCC.H264, source.Frames.Count() / (source.Frames.Sum(x => x.Item2.Milliseconds) / 1000d), new Size(ivm.Source.Mat.Width, ivm.Source.Mat.Height)))
                    {
                        foreach (var mat in (ivm.Source as FilmSource).Frames.Select(x => x.Item1))
                        {
                            videoWriter.Write(mat);
                        }
                    }
                }
                else
                {
                    ivm.Source.Mat.SaveImage(dialog.FileName);
                }
            }
        }

        private static string GetExtensions(bool gifSupport, bool mp4Support)
        {
            var ret = "Windows Bitmaps|*.bmp;*.dib|" +
                      "JPEG files|*.jpg;*.jpeg;*.jpe|" +
                      "JPEG 2000 files|*.jp2|" +
                      "Portable Network Graphics files|*.png|" +
                      "WebP|*.webp|" +
                      "Sun rasters|*.sr;*.ras|" +
                      "TIFF files|*.tiff;*.tif|" +
                      "Radiance HDR|*.hdr;*.pic|";
            if (gifSupport) ret += "GIF|*.gif|";
            if (mp4Support) ret += "MP4|*.mp4|";
            ret +=    "All Files|*.*";
            return ret;
        }

        public void AddOrActive(AnalyzingSource source)
        {
            //Active
            for (int i = 0; i < Tabs.Count; ++i)
            {
                ImageViewModel ivm = Tabs[i];

                if (ivm.Source.Equals(source))
                {
                    SelectedTabIndex = i;
                    return;
                }
            }

            //Add
            ImageViewModel item = new ImageViewModel(source, this)
            {
                Header = source.Name
            };
            source.SourceUpdated += MainWindowVM.SubWorkbenchVM.ChangeImageSource;
            Tabs.Add(item);
            SelectedTabIndex = Tabs.Count - 1;
            OnPropertyChanged("Tabs");
        }

        public void CloseTab(ImageViewModel remove)
        {
            if (Tabs.Contains(remove))
            {
                Tabs.Remove(remove);
                remove.Source.Dispose();
            }
            OnPropertyChanged("Tabs");
        }

        public int SelectedTabIndex
        {
            get { return _selectedIndex; }
            set
            {
                SetProperty<int>(ref _selectedIndex, value, "SelectedTabIndex");
                MainWindowVM.MainWindow.MainWorkbench.TabControl_Workbench.Focus();
            }
        }

        public ImageViewModel SelectedImageVM
        {
            get { return _selectedImageVM; }
            set
            {
                SetProperty<ImageViewModel>(ref _selectedImageVM, value, "SelectedImageVM");
                OnTabChanged(this, new EventArgs());
            }
        }

        public ObservableCollection<ImageViewModel> Tabs { get; set; }

        public event EventHandler TabChanged;
        protected virtual void OnTabChanged(object sender, EventArgs e)
        {
            if (TabChanged != null)
                TabChanged(sender, e);
        }
    }
}
