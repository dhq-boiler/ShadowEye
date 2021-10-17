

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ShadowEye.Model;

namespace ShadowEye.ViewModel
{
    public class ScalingDialogViewModel : CommandSink
    {
        private MainWorkbenchViewModel _ImageContainerVM;
        private AnalyzingSource _SelectedItem;
        private static int s_createCount;
        private double __Width;
        private double __Height;
        private double __WidthPercent;
        private double __HeightPercent;
        private bool _SizeIsPercent;
        private bool _KeepAspectRatio;
        private bool _ChangeFromWidth;
        private bool _ChangeFromHeight;

        private double _Width
        {
            get { return __Width; }
            set
            {
                if (value == double.NaN) return;
                if (value < 0)
                    __Width = 0;
                else if (value > double.MaxValue)
                    __Width = double.MaxValue;
                else
                    __Width = value;
            }
        }

        private double _Height
        {
            get { return __Height; }
            set
            {
                if (value == double.NaN) return;
                if (value < 0)
                    __Height = 0;
                else if (value > double.MaxValue)
                    __Height = double.MaxValue;
                else
                    __Height = value;
            }
        }

        private double _WidthPercent
        {
            get { return __WidthPercent; }
            set
            {
                if (value == double.NaN) return;
                if (value < 0)
                    __WidthPercent = 0;
                else if (value > double.MaxValue)
                    __WidthPercent = double.MaxValue;
                else
                    __WidthPercent = value;
            }
        }

        private double _HeightPercent
        {
            get { return __HeightPercent; }
            set
            {
                if (value == double.NaN) return;
                if (value < 0)
                    __HeightPercent = 0;
                else if (value > double.MaxValue)
                    __HeightPercent = double.MaxValue;
                else
                    __HeightPercent = value;
            }
        }

        public ScalingDialogViewModel(MainWorkbenchViewModel imageContainerVM)
        {
            _ImageContainerVM = imageContainerVM;
        }

        public MainWorkbenchViewModel ImageContainerVM
        {
            get { return _ImageContainerVM; }
            set { SetProperty<MainWorkbenchViewModel>(ref _ImageContainerVM, value, "ImageContainerVM"); }
        }

        public ComboBoxItem[] Items
        {
            get
            {
                return _ImageContainerVM.Tabs.Select(a => new ComboBoxItem() { Content = a.Source }).ToArray();
            }
        }

        public AnalyzingSource SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                SetProperty<AnalyzingSource>(ref _SelectedItem, value, "SelectedItem");
                if (_SelectedItem != null)
                {
                    _Width = _SelectedItem.Mat.Width;
                    _Height = _SelectedItem.Mat.Height;
                    _WidthPercent = 100;
                    _HeightPercent = 100;
                    OnPropertyChanged("Width", "Height");
                }
            }
        }

        public bool KeepAspectRatio
        {
            get { return _KeepAspectRatio; }
            set { SetProperty<bool>(ref _KeepAspectRatio, value, "KeepAspectRatio"); }
        }

        public bool SizeIsPercent
        {
            get { return _SizeIsPercent; }
            set
            {
                SetProperty<bool>(ref _SizeIsPercent, value, "SizeIsPercent", "Width", "Height");
            }
        }

        public double Width
        {
            get
            {
                if (SizeIsPercent)
                    return _WidthPercent;
                else
                    return _Width;
            }
            set
            {
                double failsafe = Width;
                try
                {
                    if (SelectedItem == null || value == double.NaN) return;
                    if (SizeIsPercent)
                    {
                        _WidthPercent = value;
                        _Width = SelectedItem.Mat.Width * _WidthPercent / 100d;
                    }
                    else
                    {
                        _Width = value;
                        _WidthPercent = 100d * _Width / SelectedItem.Mat.Width;
                    }

                    OnPropertyChanged("Width");

                    if (KeepAspectRatio)
                    {
                        _Height = SelectedItem.Mat.Height * _WidthPercent / 100d;
                        _HeightPercent = _WidthPercent;
                        if (!_ChangeFromHeight)
                        {
                            _ChangeFromWidth = true;
                            OnPropertyChanged("Height");
                            _ChangeFromWidth = false;
                        }
                    }
                }
                catch (OverflowException)
                {
                    Width = failsafe;
                }
            }
        }

        public double Height
        {
            get
            {
                if (SizeIsPercent)
                    return _HeightPercent;
                else
                    return _Height;
            }
            set
            {
                double failsafe = Height;
                try
                {
                    if (SelectedItem == null || value == double.NaN) return;
                    if (SizeIsPercent)
                    {
                        _HeightPercent = value;
                        _Height = SelectedItem.Mat.Height * _HeightPercent / 100d;
                    }
                    else
                    {
                        _Height = value;
                        _HeightPercent = 100d * _Height / SelectedItem.Mat.Height;
                    }

                    OnPropertyChanged("Height");

                    if (KeepAspectRatio)
                    {
                        _Width = SelectedItem.Mat.Width * _HeightPercent / 100d;
                        _WidthPercent = _HeightPercent;
                        if (!_ChangeFromWidth)
                        {
                            _ChangeFromHeight = true;
                            OnPropertyChanged("Width");
                            _ChangeFromHeight = false;
                        }
                    }
                }
                catch (OverflowException)
                {
                    Height = failsafe;
                }
            }
        }

        public void AddComputingTab()
        {
            try
            {
                var source = new ScaledSource(
                    string.Format("Scaled-{0}", ++s_createCount),
                    SelectedItem,
                    (int)Math.Round(_Width), (int)Math.Round(_Height));
                (App.Current.MainWindow as MainWindow).MainWindowVM.ImageContainerVM.AddOrActive(source);
            }
            catch (OpenCvSharp.OpenCVException e)
            {
                MessageBox.Show(e.ToString(), e.Message, MessageBoxButton.OK);
            }
        }
    }
}
