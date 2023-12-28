

using OpenCvSharp.WpfExtensions;
using ShadowEye.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ShadowEye.ViewModel
{
    public class ChannelExtractionDialogViewModel : CommandSink
    {
        private MainWorkbenchViewModel _ImageContainerVM;
        private AnalyzingSource _TargetImage;
        private ObservableCollection<WriteableBitmap> _Split;
        private bool _ExtractChannel0;
        private bool _ExtractChannel1;
        private bool _ExtractChannel2;
        private bool _ExtractChannel3;
        private static int s_createCount;

        public ChannelExtractionDialogViewModel(MainWorkbenchViewModel imageContainerVM)
        {
            _ImageContainerVM = imageContainerVM;
        }

        public ComboBoxItem[] Items
        {
            get
            {
                return _ImageContainerVM.Tabs.Select(a => new ComboBoxItem() { Content = a.Source }).ToArray();
            }
        }

        public AnalyzingSource TargetImage
        {
            get { return _TargetImage; }
            set
            {
                bool changed = SetProperty<AnalyzingSource>(ref _TargetImage, value, "TargetImage");
                if (changed && _TargetImage != null)
                {
                    var channels = _TargetImage.Mat.Channels();
                    if (channels > 0)
                    {
                        Split = new ObservableCollection<WriteableBitmap>();
                        var mat = _TargetImage.Mat;
                        var split = mat.Split();
                        for (int i = 0; i < channels; ++i)
                        {
                            try
                            {
                                WriteableBitmapConverter.ToWriteableBitmap(split[i], Split[i]);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Split.Add(WriteableBitmapConverter.ToWriteableBitmap(split[i]));
                            }
                            catch (ArgumentException)
                            {
                                Split.Add(WriteableBitmapConverter.ToWriteableBitmap(split[i]));
                            }
                        }
                    }
                    else
                    {
                        Debug.Fail("although channel == 0, ChannelExtractionDialog tried to extract channel.");
                    }
                }
            }
        }

        public ObservableCollection<WriteableBitmap> Split
        {
            get { return _Split; }
            set { SetProperty<ObservableCollection<WriteableBitmap>>(ref _Split, value, "Split"); }
        }

        public bool ExtractChannel0
        {
            get { return _ExtractChannel0; }
            set { SetProperty<bool>(ref _ExtractChannel0, value, "ExtractChannel0"); }
        }

        public bool ExtractChannel1
        {
            get { return _ExtractChannel1; }
            set { SetProperty<bool>(ref _ExtractChannel1, value, "ExtractChannel1"); }
        }

        public bool ExtractChannel2
        {
            get { return _ExtractChannel2; }
            set { SetProperty<bool>(ref _ExtractChannel2, value, "ExtractChannel2"); }
        }

        public bool ExtractChannel3
        {
            get { return _ExtractChannel3; }
            set { SetProperty<bool>(ref _ExtractChannel3, value, "ExtractChannel3"); }
        }

        internal void AddComputingTab()
        {
            if (ExtractChannel0 && Split.Count >= 1)
            {
                var source = new ChannelExtractedSource(string.Format("ExtCh{0}-{1}", 0, ++s_createCount), TargetImage, 0);
                _ImageContainerVM.AddOrActive(source);
            }

            if (ExtractChannel1 && Split.Count >= 2)
            {
                var source = new ChannelExtractedSource(string.Format("ExtCh{0}-{1}", 1, ++s_createCount), TargetImage, 1);
                _ImageContainerVM.AddOrActive(source);
            }

            if (ExtractChannel2 && Split.Count >= 3)
            {
                var source = new ChannelExtractedSource(string.Format("ExtCh{0}-{1}", 2, ++s_createCount), TargetImage, 2);
                _ImageContainerVM.AddOrActive(source);
            }

            if (ExtractChannel3 && Split.Count >= 4)
            {
                var source = new ChannelExtractedSource(string.Format("ExtCh{0}-{1}", 3, ++s_createCount), TargetImage, 3);
                _ImageContainerVM.AddOrActive(source);
            }
        }
    }
}
