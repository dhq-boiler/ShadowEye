

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using libimgengCore;
using ShadowEye.Model;
using ShadowEye.Utils;

namespace ShadowEye.ViewModel
{
    public class SubWorkbenchViewModel : Notifier
    {
        private MainWindowViewModel _mainwindowVM;
        private Histogram _histogram;
        private Task _task;

        public SubWorkbenchViewModel(MainWindowViewModel mainwindowVM)
        {
            _mainwindowVM = mainwindowVM;
            LevelIndicatorVM = new LevelIndicatorViewModel();
        }

        public void ChangeSourceBySwitchTab(object sender, EventArgs e)
        {
            try
            {
                MainWorkbenchViewModel imageContainerVM = sender as MainWorkbenchViewModel;
                if (imageContainerVM.SelectedImageVM != null)
                {
                    ImageViewModel imageVM = imageContainerVM.SelectedImageVM;
                    AnalyzingSource source = imageVM.Source;
                    if (source.IsEnable && source.Bitmap != null)
                    {
                        try
                        {
                            InitHistogram(source);
                            SetHistogram(source);
                        }
                        catch (Exception ex)
                        {
                            source.IsEnable = false;
                            MessageBox.Show(Properties.Resource_Localization_Messages.NotSupportFormat + "\n\nException:\n" + ex.ToString(),
                                Properties.Resource_Localization_Labels.NotSupportFormatError,
                                MessageBoxButton.OK);
                        }
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                //imageVM == null handled.
                Trace.WriteLine(ex.ToString());
            }
        }

        public void ChangeImageSource(object sender, EventArgs e)
        {
            AnalyzingSource source = sender as AnalyzingSource;
            if (source != null && source.IsEnable && source.Bitmap != null && _task == null)
            {
                _task = new Task(() =>
                    {
                        try
                        {
                            source.Bitmap.Value.Dispatcher.Invoke(() =>
                            {
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                try
                                {
                                    SetHistogram(source);
                                }
                                catch (Exception ex)
                                {
                                    source.IsEnable = false;
                                    MessageBox.Show(
                                        Properties.Resource_Localization_Messages.NotSupportFormat +
                                        "\n\nException:\n" + ex.ToString(),
                                        Properties.Resource_Localization_Labels.NotSupportFormatError,
                                        MessageBoxButton.OK);
                                }
                                finally
                                {
                                    _task = null;
                                }

                                sw.Stop();
                                HistogramFps = 1.0 / (sw.ElapsedMilliseconds / 1000.0);
                            });
                        }
                        catch (TaskCanceledException exception)
                        {
                            _task = null;
                        }
                    });
                _task.Start();
            }
        }

        private void SetHistogram(AnalyzingSource source)
        {
            if (source.Mat != null)
            {
                if (Histogram == null)
                {
                    InitHistogram(source);
                }
                Histogram.Calculate(source.Mat.Value);
            }
            else
            {
                Trace.WriteLine("Histogram.SetHistogram() failed to receive source.Mat.");
            }
        }

        private void InitHistogram(AnalyzingSource source)
        {
            if (_histogram != null)
            {
                _histogram.Dispose();
                Trace.WriteLine("Histogram disposed.");
            }
            Histogram = new Histogram();
            Histogram.PropertyChanged += (s, ea) => OnPropertyChanged();
            if (source.Mat != null)
            {
                Histogram.Initialize(source.Mat.Value, source.ChannelType);
            }
            else
            {
                Trace.WriteLine("Histogram.InitHistogram() failed to receive source.Mat.");
            }
        }

        public MainWindowViewModel MainWindowVM
        {
            get { return _mainwindowVM; }
        }


        private LevelIndicatorViewModel _LevelIndicatorVM;
        public LevelIndicatorViewModel LevelIndicatorVM
        {
            get { return _LevelIndicatorVM; }
            set { SetProperty<LevelIndicatorViewModel>(ref _LevelIndicatorVM, value, "LevelIndicatorVM"); }
        }

        public Histogram Histogram
        {
            get { return _histogram; }
            set
            {
                SetProperty<Histogram>(ref _histogram, value, "Histogram");
            }
        }

        private double _HistogramFps;
        public double HistogramFps
        {
            get { return _HistogramFps; }
            set { SetProperty<double>(ref _HistogramFps, value, "HistogramFps"); }
        }
    }
}
