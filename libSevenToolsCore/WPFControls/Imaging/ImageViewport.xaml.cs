// Copyright © 2015 dhq_boiler.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace libSevenToolsCore.WPFControls.Imaging
{
    /// <summary>
    /// Interaction logic for ImageViewport.xaml
    /// </summary>
    public partial class ImageViewport : UserControl, INotifyPropertyChanged
    {
        public ImageViewport()
        {
            InitializeComponent();

            LoadResources();
        }

        private void LoadResources()
        {
            var assembly = System.Reflection.Assembly.GetAssembly(this.GetType());
            string resourceName = assembly.GetName().Name + ".g";
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(resourceName, assembly);

            using (System.Resources.ResourceSet set = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true))
            {
                LoadUniformButtonImageResource(set);
            }
        }

        private void LoadUniformButtonImageResource(System.Resources.ResourceSet set)
        {
            using (System.IO.UnmanagedMemoryStream s = (System.IO.UnmanagedMemoryStream)set.GetObject("images/uniform.png", true))
            {
                s.Seek(0, System.IO.SeekOrigin.Begin);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = s;
                bitmap.EndInit();
                bitmap.Freeze();
                Image_Uniform.Source = bitmap;
            }
        }

        #region 依存プロパティ

        public static readonly DependencyProperty ShowTransparentGridProperty = DependencyProperty.Register("ShowTransparentGrid",
            typeof(bool),
            typeof(ImageViewport),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ImageViewport.OnShowTransparentGridChanged)));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(WriteableBitmap),
            typeof(ImageViewport),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ImageViewport.OnSourceChanged)));

        //Offset X
        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX",
            typeof(int),
            typeof(ImageViewport),
            new FrameworkPropertyMetadata(0, new PropertyChangedCallback(ImageViewport.OnOffsetXChanged), new CoerceValueCallback(ImageViewport.CoerceValueOffsetX)));

        //Offset Y
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY",
            typeof(int),
            typeof(ImageViewport),
            new FrameworkPropertyMetadata(0, new PropertyChangedCallback(ImageViewport.OnOffsetYChanged), new CoerceValueCallback(ImageViewport.CoerceValueOffsetY)));

        //Scale
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale",
            typeof(double),
            typeof(ImageViewport),
            new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(ImageViewport.OnScaleChanged), new CoerceValueCallback(ImageViewport.CoerceValueScaleFactor)));

        public static readonly DependencyProperty PixelBorderColorProperty = DependencyProperty.Register("PixelBorderColor",
            typeof(Color),
            typeof(ImageViewport),
            new FrameworkPropertyMetadata(Colors.White, new PropertyChangedCallback(ImageViewport.OnPixelBorderColorChanged)));

        public static readonly DependencyProperty InterpolationProperty = DependencyProperty.Register("Interpolation",
            typeof(Interpolation),
            typeof(ImageViewport),
            new FrameworkPropertyMetadata(Interpolation.NearestNeighbor, new PropertyChangedCallback(ImageViewport.OnInterpolationChanged)));

        public static readonly DependencyProperty RenderingBackgroundProperty = DependencyProperty.Register("RenderingBackground",
            typeof(Color),
            typeof(ImageViewport),
            new FrameworkPropertyMetadata(Color.FromArgb(255, 255 / 2, 255 / 2, 255 / 2), new PropertyChangedCallback(ImageViewport.OnRenderingBackgroundChanged)));

        #endregion 依存プロパティ

        #region ルーティングイベント

        public static readonly RoutedEvent ViewportRenderedEvent = EventManager.RegisterRoutedEvent("ViewportRendered",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageViewport));

        public static readonly RoutedEvent PixelPointedEvent = EventManager.RegisterRoutedEvent("PixelPointed",
            RoutingStrategy.Bubble, typeof(PixelPointedEventHandler), typeof(ImageViewport));

        public class PixelPointedEventArgs : RoutedEventArgs
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public int B { get; private set; }
            public int G { get; private set; }
            public int R { get; private set; }
            public int A { get; private set; }

            public PixelPointedEventArgs()
                : base()
            { }

            public PixelPointedEventArgs(RoutedEvent revent, int x, int y, int b, int g, int r)
                : base(revent)
            {
                X = x;
                Y = y;
                B = b;
                G = g;
                R = r;
            }

            public PixelPointedEventArgs(RoutedEvent revent, int x, int y, int b, int g, int r, int a)
                : this(revent, x, y, b, g, r)
            {
                A = a;
            }
        }

        public delegate void PixelPointedEventHandler(object sender, PixelPointedEventArgs e);

        #endregion ルーティングイベント

        #region CLRイベント

        public event RoutedEventHandler ViewportRendered
        {
            add { AddHandler(ViewportRenderedEvent, value); }
            remove { RemoveHandler(ViewportRenderedEvent, value); }
        }

        public event PixelPointedEventHandler PixelPointed
        {
            add { AddHandler(PixelPointedEvent, value); }
            remove { RemoveHandler(PixelPointedEvent, value); }
        }

        private void RaiseViewportRenderedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(ImageViewport.ViewportRenderedEvent);
            RaiseEvent(newEventArgs);
        }

        private void RaisePixelPointedEvent(int x, int y, int b, int g, int r, int a)
        {
            PixelPointedEventArgs newEventArgs = new PixelPointedEventArgs(ImageViewport.PixelPointedEvent, x, y, b, g, r, a);
            RaiseEvent(newEventArgs);
        }

        #endregion CLRイベント

        #region 依存プロパティコールバック

        private static void OnShowTransparentGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageViewport ctrl = d as ImageViewport;
            if (ctrl != null)
            {
            }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageViewport ctrl = d as ImageViewport;

            if (ctrl != null)
            {
                if (ctrl.Source != null)
                {
                    ctrl.Source.Changed += ctrl.Source_Changed;
                    ctrl.Render();
                }
            }
        }

        private void Source_Changed(object sender, EventArgs e)
        {
            Render(false);
        }

        public void Uniform()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    double renderingAreaWidth = RenderingAreaWidth;
                    double renderingAreaHeight = RenderingAreaHeight;

                    int width_s = SourceWidth;
                    int height_s = SourceHeight;
                    double width_r = renderingAreaWidth;
                    double height_r = renderingAreaHeight;
                    double widthProportion = width_s / width_r;
                    double heightProportion = height_s / height_r;

                    LockRendering();

                    if (widthProportion > heightProportion)
                    {
                        //横方向にFit
                        double a = width_r / width_s;
                        ScaleFactor = a;
                        OffsetX = 0;
                        OffsetY = (int)(-(renderingAreaHeight - ScaledSourceHeight) / 2);
                    }
                    else
                    {
                        //縦方向にFit
                        double a = height_r / height_s;
                        ScaleFactor = a;
                        OffsetY = 0;
                        OffsetX = (int)(-(renderingAreaWidth - ScaledSourceWidth) / 2);
                    }

                    UnlockRendering();
                }));
        }

        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageViewport ctrl = d as ImageViewport;
            if (ctrl != null)
            {
                ctrl.Render();
            }
        }

        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageViewport ctrl = d as ImageViewport;
            if (ctrl != null)
            {
                ctrl.Render();
            }
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageViewport ctrl = d as ImageViewport;
            if (ctrl != null)
            {
                ctrl.UpdateScrollBarRange();
                ctrl.Render();
            }
        }

        private static void OnPixelBorderColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageViewport ctrl = d as ImageViewport;
            if (ctrl != null)
            {
                ctrl.Render(true);
            }
        }

        private static void OnInterpolationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageViewport ctrl = d as ImageViewport;
            if (ctrl != null)
            {
                ctrl.Render(true);
            }
        }

        private static void OnRenderingBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageViewport ctrl = d as ImageViewport;
            if (ctrl != null)
            {
                ctrl.Render(true);
            }
        }

        #endregion 依存プロパティコールバック

        #region Coerce Value Callback

        private static object CoerceValueOffsetX(DependencyObject d, object baseValue)
        {
            ImageViewport ctrl = d as ImageViewport;
            int current = (int)baseValue;
            if (current < ctrl.MinimumOffsetX) current = ctrl.MinimumOffsetX;
            if (current > ctrl.MaximumOffsetX) current = ctrl.MaximumOffsetX;
            return current;
        }

        private static object CoerceValueOffsetY(DependencyObject d, object baseValue)
        {
            ImageViewport ctrl = d as ImageViewport;
            int current = (int)baseValue;
            if (current < ctrl.MinimumOffsetY) current = ctrl.MinimumOffsetY;
            if (current > ctrl.MaximumOffsetY) current = ctrl.MaximumOffsetY;
            return current;
        }

        private static object CoerceValueScaleFactor(DependencyObject d, object baseValue)
        {
            ImageViewport ctrl = d as ImageViewport;
            double current = (double)baseValue;
            if (current < ctrl.MinimumScaleFactor) current = ctrl.MinimumScaleFactor;
            if (current > ctrl.MaximumScaleFactor) current = ctrl.MaximumScaleFactor;
            return current;
        }

        #endregion Coerce Value Callback

        #region CLRプロパティ

        public bool ShowTransparentGrid
        {
            get { return (bool)GetValue(ShowTransparentGridProperty); }
            set { SetValue(ShowTransparentGridProperty, value); }
        }

        #region Rendering

        #region WriteableBitmap

        public WriteableBitmap Source
        {
            get { return (WriteableBitmap)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private WriteableBitmap _RenderBitmap;
        public WriteableBitmap RenderBitmap
        {
            get { return _RenderBitmap; }
            set { SetProperty<WriteableBitmap>(ref _RenderBitmap, value); }
        }

        #endregion WriteableBitmap

        #region Parameters for Rendering

        /// <summary>
        /// オリジナル画像のサイズ．
        /// </summary>
        public Size SourceSize
        {
            get { return new Size(SourceWidth, SourceHeight); }
        }

        /// <summary>
        /// オリジナル画像の幅．
        /// </summary>
        public int SourceWidth
        {
            get { return Source != null ? Source.PixelWidth : 0; }
        }

        /// <summary>
        /// オリジナル画像の高さ．
        /// </summary>
        public int SourceHeight
        {
            get { return Source != null ? Source.PixelHeight : 0; }
        }

        /// <summary>
        /// 拡縮されたオリジナル画像のサイズ．
        /// </summary>
        public Size ScaledSourceSize
        {
            get { return new Size(ScaledSourceWidth, ScaledSourceHeight); }
        }

        /// <summary>
        /// 拡縮されたオリジナル画像の幅．
        /// </summary>
        public int ScaledSourceWidth
        {
            get { return (int)(SourceWidth * ScaleFactor); }
        }

        /// <summary>
        /// 拡縮されたオリジナル画像の高さ．
        /// </summary>
        public int ScaledSourceHeight
        {
            get { return (int)(SourceHeight * ScaleFactor); }
        }

        /// <summary>
        /// レンダリングエリアのサイズ．
        /// </summary>
        public Size RenderingAreaSize
        {
            get { return new Size(RenderingAreaWidth, RenderingAreaHeight); }
        }

        /// <summary>
        /// レンダリングエリアの幅．
        /// </summary>
        public int RenderingAreaWidth
        {
            get { return (int)Grid_RenderArea.ActualWidth; }
        }

        /// <summary>
        /// レンダリングエリアの高さ．
        /// </summary>
        public int RenderingAreaHeight
        {
            get { return (int)Grid_RenderArea.ActualHeight; }
        }

        /// <summary>
        /// オリジナル画像における描画開始点．
        /// </summary>
        public Point SourceOffsetPoint
        {
            get { return new Point(OffsetX, OffsetY); }
        }

        /// <summary>
        /// オリジナル画像における描画開始位置のX座標．
        /// レンダリングエリアにオリジナル画像の一部または全部が必ず含まれる．
        /// </summary>
        public int OffsetX
        {
            get { return (int)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        /// <summary>
        /// オリジナル画像における描画開始位置のY座標．
        /// レンダリングエリアにオリジナル画像の一部または全部が必ず含まれる．
        /// </summary>
        public int OffsetY
        {
            get { return (int)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        /// <summary>
        /// スケールファクター．
        /// factor == 1.0 : オリジナルサイズ，factor &lt; 1.0 : 縮小，factor &gt; 1.0 : 拡大を表す．
        /// </summary>
        public double ScaleFactor
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        private int MinimumOffsetY
        {
            get { return -RenderingAreaHeight; }
        }

        private int MaximumOffsetY
        {
            get { return ScaledSourceHeight; }
        }

        private int MinimumOffsetX
        {
            get { return -RenderingAreaWidth; }
        }

        private int MaximumOffsetX
        {
            get { return ScaledSourceWidth; }
        }

        private double MinimumScaleFactor
        {
            get { return 0.005; }
        }

        private double MaximumScaleFactor
        {
            get { return 32.0; }
        }

        public Interpolation Interpolation
        {
            get { return (Interpolation)GetValue(InterpolationProperty); }
            set { SetValue(InterpolationProperty, value); }
        }

        public Color RenderingBackground
        {
            get { return (Color)GetValue(RenderingBackgroundProperty); }
            set { SetValue(RenderingBackgroundProperty, value); }
        }

        #region Expanding

        /// <summary>
        /// 画素境界線の幅．
        /// </summary>
        public int PixelBorderThickness
        {
            get
            {
                if (ScaleFactor >= 10)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 画素境界線の色．
        /// </summary>
        public Color PixelBorderColor
        {
            get { return (Color)GetValue(PixelBorderColorProperty); }
            set { SetValue(PixelBorderColorProperty, value); }
        }

        #endregion Expanding

        #endregion Parameters for Rendering

        #region Calculated Parameters

        /// <summary>
        /// １画素当たりの描画サイズ．
        /// </summary>
        public double RenderingSizePerPixel
        {
            get { return ScaleFactor; }
        }

        /// <summary>
        /// ビューポートの位置．
        /// </summary>
        public Rect ViewportRect
        {
            get { return new Rect(OffsetX / ScaleFactor, OffsetY / ScaleFactor, RenderingAreaWidth / ScaleFactor, RenderingAreaHeight / ScaleFactor); }
        }

        /// <summary>
        /// レンダリングエリアにおける画像の描画領域．
        /// </summary>
        public Rect RenderedRenderingAreaRect
        {
            get
            {
                try
                {
                    var rect = new Rect(new Point(BeginRenderingOffsetX, BeginRenderingOffsetY),
                                        new Point(EndRenderingOffsetX, EndRenderingOffsetY));
                    return rect;
                }
                catch (NotIncludeException)
                {
                    throw;
                }
            }
        }

        private Rect OldRenderedRenderingAreaRect { get; set; }

        /// <summary>
        /// レンダリングエリアにおける画像の描画開始位置のX座標．
        /// </summary>
        public int BeginRenderingOffsetX
        {
            get
            {
                var renderingLeft = 0;
                var renderingRight = RenderingAreaWidth;
                var sourceLeft = -OffsetX;
                var sourceRight = sourceLeft + ScaledSourceWidth;

                if (OffsetX < 0)
                {
                    if (renderingRight <= sourceLeft)
                    {
                        throw new NotIncludeException();
                    }
                    else
                    {
                        return sourceLeft;
                    }
                }
                else
                {
                    if (sourceRight <= renderingLeft)
                    {
                        throw new NotIncludeException();
                    }
                    else
                    {
                        return renderingLeft;
                    }
                }
            }
        }

        /// <summary>
        /// レンダリングエリアにおける画像の描画開始位置のY座標．
        /// </summary>
        public int BeginRenderingOffsetY
        {
            get
            {
                var renderingTop = 0;
                var renderingBottom = RenderingAreaHeight;
                var sourceTop = -OffsetY;
                var sourceBottom = sourceTop + ScaledSourceHeight;

                if (OffsetY < 0)
                {
                    if (renderingBottom <= sourceTop)
                    {
                        throw new NotIncludeException();
                    }
                    else
                    {
                        return sourceTop;
                    }
                }
                else
                {
                    if (sourceBottom <= renderingTop)
                    {
                        throw new NotIncludeException();
                    }
                    else
                    {
                        return renderingTop;
                    }
                }
            }
        }

        /// <summary>
        /// レンダリングエリアにおける画像の描画終了位置のX座標．
        /// </summary>
        public int EndRenderingOffsetX
        {
            get
            {
                var renderingLeft = 0;
                var renderingRight = RenderingAreaWidth;
                var sourceLeft = -OffsetX;
                var sourceRight = sourceLeft + ScaledSourceWidth;
                if (OffsetX < 0)
                {
                    if (renderingRight <= sourceLeft)
                    {
                        throw new NotIncludeException();
                    }
                    if (sourceRight < renderingRight)
                    {
                        return sourceRight;
                    }
                    else
                    {
                        return renderingRight;
                    }
                }
                else
                {
                    if (sourceRight <= renderingLeft)
                    {
                        throw new NotIncludeException();
                    }
                    else if (renderingRight < sourceRight)
                    {
                        return renderingRight;
                    }
                    else
                    {
                        return sourceRight;
                    }
                }
            }
        }

        /// <summary>
        /// レンダリングエリアにおける画像の描画終了位置のY座標．
        /// </summary>
        public int EndRenderingOffsetY
        {
            get
            {
                var renderingTop = 0;
                var renderingBottom = RenderingAreaHeight;
                var sourceTop = -OffsetY;
                var sourceBottom = sourceTop + ScaledSourceHeight;
                if (OffsetY < 0)
                {
                    if (renderingBottom <= sourceTop)
                    {
                        throw new NotIncludeException();
                    }
                    if (sourceBottom < renderingBottom)
                    {
                        return sourceBottom;
                    }
                    else
                    {
                        return renderingBottom;
                    }
                }
                else
                {
                    if (sourceBottom <= renderingTop)
                    {
                        throw new NotIncludeException();
                    }
                    else if (renderingBottom < sourceBottom)
                    {
                        return renderingBottom;
                    }
                    else
                    {
                        return sourceBottom;
                    }
                }
            }
        }

        /// <summary>
        /// オリジナル画像の描画範囲．
        /// </summary>
        public Rect RenderedSourceRect
        {
            get
            {
                return new Rect(OffsetX / ScaleFactor, OffsetY / ScaleFactor,
                                RenderedWidth / ScaleFactor, RenderedHeight / ScaleFactor);
            }
        }

        /// <summary>
        /// オリジナル画像の描画される幅．
        /// </summary>
        public int RenderedWidth
        {
            get
            {
                var renderingLeft = 0;
                var renderingRight = RenderingAreaWidth;
                var sourceLeft = -OffsetX;
                var sourceRight = sourceLeft + ScaledSourceWidth;

                if (OffsetX < 0)
                {
                    if (renderingRight <= sourceLeft)
                    {
                        throw new NotIncludeException();
                    }
                    else if (sourceRight < renderingRight)
                    {
                        return sourceRight - sourceLeft;
                    }
                    else
                    {
                        return renderingRight - sourceLeft;
                    }
                }
                else
                {
                    if (sourceRight <= renderingLeft)
                    {
                        throw new NotIncludeException();
                    }
                    else if (renderingRight < sourceRight)
                    {
                        return renderingRight;
                    }
                    else
                    {
                        return sourceRight;
                    }
                }
            }
        }

        /// <summary>
        /// オリジナル画像の描画される幅．
        /// </summary>
        public int RenderedHeight
        {
            get
            {
                var renderingTop = 0;
                var renderingBottom = RenderingAreaHeight;
                var sourceTop = -OffsetY;
                var sourceBottom = sourceTop + ScaledSourceHeight;

                if (OffsetY < 0)
                {
                    if (renderingBottom <= sourceTop)
                    {
                        throw new NotIncludeException();
                    }
                    else if (sourceBottom < renderingBottom)
                    {
                        return sourceBottom - sourceTop;
                    }
                    else
                    {
                        return renderingBottom - sourceTop;
                    }
                }
                else
                {
                    if (sourceBottom <= renderingTop)
                    {
                        throw new NotIncludeException();
                    }
                    else if (renderingBottom < sourceBottom)
                    {
                        return renderingBottom;
                    }
                    else
                    {
                        return sourceBottom;
                    }
                }
            }
        }

        #region スクロールバー

        public double VerticalMinValue
        {
            get { return MinimumOffsetY; }
        }

        public double VerticalMaxValue
        {
            get { return MaximumOffsetY; }
        }

        public double HorizontalMinValue
        {
            get { return MinimumOffsetX; }
        }

        public double HorizontalMaxValue
        {
            get { return MaximumOffsetX; }
        }

        #endregion スクロールバー

        #endregion Calculated Parameters

        #endregion Rendering

        #endregion CLR プロパティ

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (var propertyName in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (value == null || (storage != null && storage.Equals(value)))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion INotifyPropertyChanged

        private int _LockRenderingCount;

        private void LockRendering()
        {
            ++_LockRenderingCount;
        }

        private void UnlockRendering(bool updateEntirely = false)
        {
            if (_LockRenderingCount > 0 && --_LockRenderingCount == 0)
            {
                Render(updateEntirely);
            }
        }

        private void Render(bool updateEntirely = false)
        {
            if (Source == null || RenderingAreaWidth < 1.0 || RenderingAreaHeight < 1.0 || _LockRenderingCount > 0)
                return;

            WriteableBitmap bitmap = PrepareBitmap();

            unsafe
            {
                byte* p_s = GetSourcePointer();
                byte* p_d = GetDestinationPointer(bitmap);

                SetInterpolationMethod(out Interpolation method);
                SetRenderingBackground(out Color renderingBackground);

                try
                {
                    bitmap.Lock();

                    Rect renderedRenderingAreaRect = RenderedRenderingAreaRect;
                    if (ScaleFactor == 1.0)
                    {
                        RenderAtScaleOne(bitmap, p_s, p_d, renderingBackground, method, renderedRenderingAreaRect);
                    }
                    else
                    {
                        RenderAtDifferentScale(bitmap, p_s, p_d, renderingBackground, method,
                            renderedRenderingAreaRect);
                    }

                    FinalizeRendering(bitmap, updateEntirely, renderedRenderingAreaRect);
                }
                catch (NotIncludeException)
                {
                    HandleNotIncludeException(bitmap, renderingBackground);
                }
            }

            FinalizeBitmap(bitmap);
        }

        private void FinalizeBitmap(WriteableBitmap bitmap)
        {
            bitmap.Unlock();
            RenderBitmap = bitmap;
            RaiseViewportRenderedEvent();
        }

        private void FinalizeRendering(WriteableBitmap bitmap, bool updateEntirely, Rect renderedRenderingAreaRect)
        {
            if (PixelBorderThickness > 0 || updateEntirely)
            {
                AddDirtyRectEntireArea(bitmap);
                UpdateOldRenderedRenderingAreaRect();
            }
            else
            {
                Rect dirtyRect = CorrectRect(WrapRects(renderedRenderingAreaRect, OldRenderedRenderingAreaRect), new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                bitmap.AddDirtyRect(new Int32Rect((int)(dirtyRect.Left + 0.5), (int)(dirtyRect.Top + 0.5), (int)(dirtyRect.Width + 0.5), (int)(dirtyRect.Height + 0.5)));
                UpdateOldRenderedRenderingAreaRect();
            }
        }

        private void SetRenderingBackground(out Color color)
        {
            color = new Color();
            color = Dispatcher.Invoke(() => RenderingBackground);
        }

        private void SetInterpolationMethod(out Interpolation interpolation)
        {
            interpolation = Imaging.Interpolation.NearestNeighbor;
            interpolation = Dispatcher.Invoke(() => Interpolation);
        }

        private unsafe byte* GetDestinationPointer(WriteableBitmap bitmap)
        {
            return (byte*)bitmap.BackBuffer.ToPointer();
        }

        private unsafe void RenderAtDifferentScale(WriteableBitmap bitmap, byte* p_s, byte* p_d, Color renderingBackground, Interpolation method, Rect renderedRenderingAreaRect)
        {
            int RenderingArea_BeginRenderingTop = (int)renderedRenderingAreaRect.Top;
            int RenderingArea_BeginRenderingLeft = (int)renderedRenderingAreaRect.Left;
            int RenderingArea_EndRenderingBottom = (int)renderedRenderingAreaRect.Bottom;
            int RenderingArea_EndRenderingRight = (int)renderedRenderingAreaRect.Right;

            int width_s = Source.PixelWidth;
            int height_s = Source.PixelHeight;
            int step_s = Source.BackBufferStride;
            int channels = GetChannels(Source.Format);
            int width_d = bitmap.PixelWidth;
            int height_d = bitmap.PixelHeight;
            int step_d = bitmap.BackBufferStride;
            int offsetX = OffsetX;
            int offsetY = OffsetY;

            double patternWidth = (double)RenderingSizePerPixel;
            double patternHeight = (double)RenderingSizePerPixel;
            int pixelBorderThickness = (int)PixelBorderThickness;
            int renderingSizePerPixel = (int)RenderingSizePerPixel;
            int sourceLeft = (int)-OffsetX;
            int sourceTop = (int)-OffsetY;
            Color pixelBorderColor = PixelBorderColor;

            int x_begin = 0;
            int x_end = ScaledSourceWidth;
            int y_begin = 0;
            int y_end = ScaledSourceHeight;

            int rx1 = 0;
            int ry1 = 0;
            int rx2 = width_s;
            int ry2 = height_s;

            int w1 = rx2 - rx1;
            int h1 = ry2 - ry1;
            int w2 = x_end - x_begin;
            int h2 = y_end - y_begin;

            Parallel.For(0, height_d, (y) =>
            {
                if (RenderingArea_BeginRenderingTop <= y && y < RenderingArea_EndRenderingBottom)
                {
                    int py = (int)((y - sourceTop) % patternHeight);
                    double fy = (double)(y - py - y_begin + offsetY) * h1 / (double)h2 + ry1;
                    int indexY = (int)((offsetY + y) / patternHeight);

                    if (pixelBorderThickness > 0 && ((indexY == 0 && py == 0) || py >= renderingSizePerPixel - pixelBorderThickness)) //Y境界線上または最上行の上端
                    {
                        for (int x = 0; x < width_d; ++x)
                        {
                            if (RenderingArea_BeginRenderingLeft <= 0 && x < RenderingArea_EndRenderingRight)
                            {
                                int px = (int)((x - sourceLeft) % patternWidth);
                                int indexX = (int)((offsetX + x) / patternWidth);
                                double fx = (double)(x - px - x_begin + offsetX) * w1 / (double)w2 + rx1;

                                //描画
                                DrawPixelBorderByAverage(p_s, width_s, height_s, step_s, channels, p_d, step_d, method, pixelBorderColor, y, fy, x, fx);
                            }
                            else
                            {
                                DrawBackgroundPixel(renderingBackground, channels, p_d, step_d, y, x);
                            }
                        }
                    }
                    else //Y画素内
                    {
                        for (int x = 0; x < width_d; ++x)
                        {
                            if (RenderingArea_BeginRenderingLeft <= x && x < RenderingArea_EndRenderingRight)
                            {
                                int px = (int)((x - sourceLeft) % patternWidth);
                                int indexX = (int)((offsetX + x) / patternWidth);
                                double fx = (double)(x - px - x_begin + offsetX) * w1 / (double)w2 + rx1;

                                if (pixelBorderThickness > 0 && ((indexX == 0 && px == 0) || px >= renderingSizePerPixel - pixelBorderThickness)) //X境界線上または最左列の左端
                                {
                                    DrawPixelBorderByAverage(p_s, width_s, height_s, step_s, channels, p_d, step_d, method, pixelBorderColor, y, fy, x, fx);
                                }
                                else //X画素内
                                {
                                    DrawForegroundPixel(renderingBackground, p_s, width_s, height_s, step_s, channels, p_d, step_d, method, y, fy, x, fx);
                                }
                            }
                            else //X画像範囲外
                            {
                                DrawBackgroundPixel(renderingBackground, channels, p_d, step_d, y, x);
                            }
                        }
                    }
                }
                else //Y画像範囲外
                {
                    for (int x = 0; x < width_d; ++x)
                    {
                        DrawBackgroundPixel(renderingBackground, channels, p_d, step_d, y, x);
                    }
                }
            });
        }

        private WriteableBitmap PrepareBitmap()
        {
            WriteableBitmap bitmap = RenderBitmap;
            if (bitmap == null || bitmap.PixelWidth != (int)RenderingAreaWidth || bitmap.PixelHeight != (int)RenderingAreaHeight || bitmap.Format != Source.Format)
            {
                bitmap = new WriteableBitmap((int)RenderingAreaWidth, (int)RenderingAreaHeight, 92, 92, Source.Format, null);
            }
            return bitmap;
        }

        private unsafe byte* GetSourcePointer()
        {
            return (byte*)Source.BackBuffer.ToPointer();
        }

        private unsafe void RenderAtScaleOne(WriteableBitmap bitmap, byte* p_s, byte* p_d, Color renderingBackground, Interpolation method, Rect renderedRenderingAreaRect)
        {
            int RenderingArea_BeginRenderingTop = (int)renderedRenderingAreaRect.Top;
            int RenderingArea_BeginRenderingLeft = (int)renderedRenderingAreaRect.Left;
            int RenderingArea_EndRenderingBottom = (int)renderedRenderingAreaRect.Bottom;
            int RenderingArea_EndRenderingRight = (int)renderedRenderingAreaRect.Right;

            int width_s = Source.PixelWidth;
            int height_s = Source.PixelHeight;
            int step_s = Source.BackBufferStride;
            int channels = GetChannels(Source.Format);
            int width_d = bitmap.PixelWidth;
            int height_d = bitmap.PixelHeight;
            int step_d = bitmap.BackBufferStride;
            int offsetX = OffsetX;
            int offsetY = OffsetY;

            // スケールファクターが1.0の場合のレンダリングロジック
            // bitmap, p_s, p_d, renderingBackground, method, renderedRenderingAreaRect を使用
            Parallel.For(0, height_d, (y) =>
            {
                if (RenderingArea_BeginRenderingTop <= y && y < RenderingArea_EndRenderingBottom)
                {
                    for (int x = 0; x < width_d; ++x)
                    {
                        if (RenderingArea_BeginRenderingLeft <= x && x < RenderingArea_EndRenderingRight)
                        {
                            for (int c = 0; c < channels; ++c)
                            {
                                *(p_d + y * step_d + x * channels + c) = *(p_s + (offsetY + y) * step_s + (offsetX + x) * channels + c);
                            }
                        }
                        else
                        {
                            DrawBackgroundPixel(renderingBackground, channels, p_d, step_d, y, x);
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < width_d; ++x)
                    {
                        DrawBackgroundPixel(renderingBackground, channels, p_d, step_d, y, x);
                    }
                }
            });
        }

        private unsafe void HandleNotIncludeException(WriteableBitmap bitmap, Color renderingBackground)
        {
            int channels = GetChannels(Source.Format);
            int step_d = bitmap.BackBufferStride;
            int width_d = bitmap.PixelWidth;
            int height_d = bitmap.PixelHeight;
            byte* p_d = (byte*)bitmap.BackBuffer.ToPointer();
            // bitmapとrenderingBackgroundを使用して例外処理を行う
            try
            {
                bitmap.Lock();

                for (int y = 0; y < height_d; ++y)
                {
                    for (int x = 0; x < width_d; ++x)
                    {
                        DrawBackgroundPixel(renderingBackground, channels, p_d, step_d, y, x);
                    }
                }
                if (PixelBorderThickness > 0)
                {
                    AddDirtyRectEntireArea(bitmap);
                }
                else
                {
                    var dirtyRect = CorrectRect(OldRenderedRenderingAreaRect, new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                    bitmap.AddDirtyRect(new Int32Rect((int)dirtyRect.Left, (int)dirtyRect.Top, (int)dirtyRect.Width, (int)dirtyRect.Height));
                }
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        // Example of breaking down into smaller functions
        private bool ShouldReturnEarly()
        {
            return Source == null || RenderingAreaWidth < 1.0 || RenderingAreaHeight < 1.0 || _LockRenderingCount > 0;
        }

        unsafe private static void DrawForegroundPixel(Color background, byte* p_s, int width_s, int height_s, int step_s, int channels, byte* p_d, int step_d, Interpolation method, int y, double fy, int x, double fx)
        {
            int p0 = 0, p1 = 0, p2 = 0, p3 = 0;
            bool success = false;

            // Perform interpolation based on the number of channels
            if (channels == 4)
            {
                success = Scaler.Interpolate(method, p_s, fx, fy, 0, width_s - 1, 0, height_s - 1, step_s, channels, out p0, out p1, out p2, out p3);
            }
            else if (channels == 3)
            {
                success = Scaler.Interpolate(method, p_s, fx, fy, 0, width_s - 1, 0, height_s - 1, step_s, channels, out p0, out p1, out p2);
            }
            else if (channels == 1)
            {
                success = Scaler.Interpolate(method, p_s, fx, fy, 0, width_s - 1, 0, height_s - 1, step_s, channels, out p0);
                p1 = p2 = p3 = p0; // Optional, based on how you want to handle single-channel images
            }

            // Set pixel values based on success of interpolation
            byte* pixel = p_d + y * step_d + x * channels;
            if (success)
            {
                pixel[0] = (byte)p0;
                if (channels > 1)
                {
                    pixel[1] = (byte)p1;
                    pixel[2] = (byte)p2;
                }
                if (channels == 4)
                {
                    pixel[3] = (byte)p3;
                }
            }
            else
            {
                pixel[0] = channels == 1 ? Math.Max(background.B, Math.Max(background.G, background.R)) : background.B;
                if (channels > 1)
                {
                    pixel[1] = background.G;
                    pixel[2] = background.R;
                }
                if (channels == 4)
                {
                    pixel[3] = background.A;
                }
            }
        }


        unsafe private static void DrawBackgroundPixel(Color background, int channels, byte* p_d, int step_d, int y, int x)
        {
            if (channels == 4)
            {
                DrawBackgroundPixelBGRWithAlpha(background, p_d, step_d, y, x);
            }
            else if (channels == 3)
            {
                DrawBackgroundPixelBGR(background, p_d, step_d, y, x);
            }
            else if (channels == 1)
            {
                *(p_d + y * step_d + x) = Math.Max(background.B, Math.Max(background.G, background.R));
            }
        }

        unsafe private static void DrawBackgroundPixelBGR(Color background, byte* p_d, int step_d, int y, int x)
        {
            *(p_d + y * step_d + x * 3 + 0) = background.B;
            *(p_d + y * step_d + x * 3 + 1) = background.G;
            *(p_d + y * step_d + x * 3 + 2) = background.R;
        }

        private int GetChannels(PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormats.BlackWhite
                || pixelFormat == PixelFormats.Gray16
                || pixelFormat == PixelFormats.Gray2
                || pixelFormat == PixelFormats.Gray32Float
                || pixelFormat == PixelFormats.Gray4
                || pixelFormat == PixelFormats.Gray8)
            {
                return 1;
            }
            else if (pixelFormat == PixelFormats.Bgr101010
                || pixelFormat == PixelFormats.Bgr24
                || pixelFormat == PixelFormats.Bgr32
                || pixelFormat == PixelFormats.Bgr555
                || pixelFormat == PixelFormats.Bgr565
                || pixelFormat == PixelFormats.Rgb128Float
                || pixelFormat == PixelFormats.Rgb24
                || pixelFormat == PixelFormats.Rgb48)
            {
                return 3;
            }
            else if (pixelFormat == PixelFormats.Bgra32
                || pixelFormat == PixelFormats.Cmyk32
                || pixelFormat == PixelFormats.Pbgra32
                || pixelFormat == PixelFormats.Prgba128Float
                || pixelFormat == PixelFormats.Prgba64
                || pixelFormat == PixelFormats.Rgba128Float
                || pixelFormat == PixelFormats.Rgba64)
            {
                return 4;
            }
            else
            {
                throw new NotSupportedException(string.Format("PixelFormat:{0}には対応していません．", pixelFormat));
            }
        }

        unsafe private static void DrawBackgroundPixelBGRWithAlpha(Color renderingBackground, byte* p_d, int step_d, int y, int x)
        {
            *(p_d + y * step_d + x * 4 + 0) = renderingBackground.B;
            *(p_d + y * step_d + x * 4 + 1) = renderingBackground.G;
            *(p_d + y * step_d + x * 4 + 2) = renderingBackground.R;
            *(p_d + y * step_d + x * 4 + 3) = renderingBackground.A;
        }

        unsafe private static void DrawPixelBorderByAverage(byte* p_s, int width_s, int height_s, int step_s, int channels, byte* p_d, int step_d, Interpolation method, Color pixelBorderColor, int y, double fy, int x, double fx)
        {
            if (channels == 4)
            {
                int p0, p1, p2, p3;
                if (Scaler.Interpolate(method, p_s, fx, fy, 0, width_s - 1, 0, height_s - 1, step_s, 4, out p0, out p1, out p2, out p3))
                {
                    *(p_d + y * step_d + x * channels) = (byte)((p0 + pixelBorderColor.B) / 2d);
                    *(p_d + y * step_d + x * channels + 1) = (byte)((p1 + pixelBorderColor.G) / 2d);
                    *(p_d + y * step_d + x * channels + 2) = (byte)((p2 + pixelBorderColor.R) / 2d);
                    *(p_d + y * step_d + x * channels + 3) = (byte)((p3 + pixelBorderColor.A) / 2d);
                }
            }
            else if (channels == 3)
            {
                //描画
                int p0, p1, p2;
                if (Scaler.Interpolate(method, p_s, fx, fy, 0, width_s - 1, 0, height_s - 1, step_s, channels, out p0, out p1, out p2))
                {
                    *(p_d + y * step_d + x * channels) = (byte)((p0 + pixelBorderColor.B) / 2d);
                    *(p_d + y * step_d + x * channels + 1) = (byte)((p1 + pixelBorderColor.G) / 2d);
                    *(p_d + y * step_d + x * channels + 2) = (byte)((p2 + pixelBorderColor.R) / 2d);
                }
            }
            else if (channels == 1)
            {
                //描画
                int p0;
                if (Scaler.Interpolate(method, p_s, fx, fy, 0, width_s - 1, 0, height_s - 1, step_s, channels, out p0))
                {
                    *(p_d + y * step_d + x * channels) = (byte)((p0 + pixelBorderColor.B) / 2d);
                }
            }
        }

        private void AddDirtyRectEntireArea(WriteableBitmap bitmap)
        {
            bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
        }

        /// <summary>
        /// 対象矩形領域を基準矩形領域内に収めた矩形領域を返します
        /// </summary>
        /// <param name="target">対象矩形領域</param>
        /// <param name="basis">基準矩形領域</param>
        /// <returns></returns>
        private Rect CorrectRect(Rect target, Rect basis)
        {
            var left = target.Left < basis.Left ? basis.Left : target.Left;
            var top = target.Top < basis.Top ? basis.Top : target.Top;
            var right = basis.Right < target.Right ? basis.Right : target.Right;
            var bottom = basis.Bottom < target.Bottom ? basis.Bottom : target.Bottom;
            return new Rect(new Point(left, top), new Point(right, bottom));
        }

        /// <summary>
        /// 2つの矩形領域を包む1つの矩形領域を返します
        /// </summary>
        /// <param name="a">矩形領域</param>
        /// <param name="b">矩形領域</param>
        /// <returns></returns>
        private Rect WrapRects(Rect a, Rect b)
        {
            var left = a.Left < b.Left ? a.Left : b.Left;
            var top = a.Top < b.Top ? a.Top : b.Top;
            var right = a.Right > b.Right ? a.Right : b.Right;
            var bottom = a.Bottom > b.Bottom ? a.Bottom : b.Bottom;
            return new Rect(new Point(left, top), new Point(right, bottom));
        }

        private void Grid_RenderArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateScrollBarRange();
            Render(true);
        }

        private void UpdateScrollBarRange()
        {
            OnPropertyChanged("VerticalMinValue");
            OnPropertyChanged("VerticalMaxValue");
            OnPropertyChanged("HorizontalMinValue");
            OnPropertyChanged("HorizontalMaxValue");
            OnPropertyChanged("RenderingSizePerPixel");
        }

        private void UpdateOldRenderedRenderingAreaRect()
        {
            try
            {
                OldRenderedRenderingAreaRect = RenderedRenderingAreaRect;
            }
            catch (NotIncludeException)
            { }
        }

        private void TextBox_Scale_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Image_RenderArea.Focus();
                    var expression = TextBox_Scale.GetBindingExpression(TextBox.TextProperty);
                    expression.UpdateSource();
                    break;
                default:
                    break;
            }
        }

        private void TextBox_Scale_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox_Scale.Select(0, TextBox_Scale.Text.Length - 1);
        }

        private Point? _PreviousMousePointerPosition;

        private void Image_RenderArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                _PreviousMousePointerPosition = e.GetPosition(sender as Image);
            }
        }

        private void Image_RenderArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _PreviousMousePointerPosition = null;
        }

        private void Image_RenderArea_MouseMove(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            var sourceSize = ScaledSourceSize;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(image);
                if (_PreviousMousePointerPosition.HasValue)
                {
                    int diffX = (int)(_PreviousMousePointerPosition.Value.X - position.X);
                    int diffY = (int)(_PreviousMousePointerPosition.Value.Y - position.Y);

                    LockRendering();

                    OffsetX += diffX;
                    OffsetY += diffY;

                    UnlockRendering();
                }
                _PreviousMousePointerPosition = position;
            }
            else
            {
                var position = e.GetPosition(image);
                var renderingPoint = GetRenderingPoint(position);
                if (renderingPoint.X >= 0
                    && renderingPoint.X < sourceSize.Width
                    && renderingPoint.Y >= 0
                    && renderingPoint.Y < sourceSize.Height)
                {
                    int b = GetValue((int)renderingPoint.X, (int)renderingPoint.Y, 0);
                    int g = GetValue((int)renderingPoint.X, (int)renderingPoint.Y, 1);
                    int r = GetValue((int)renderingPoint.X, (int)renderingPoint.Y, 2);
                    int a = GetValue((int)renderingPoint.X, (int)renderingPoint.Y, 3);
                    RaisePixelPointedEvent((int)(renderingPoint.X / ScaleFactor), (int)(renderingPoint.Y / ScaleFactor), b, g, r, a);
                }
            }
        }

        private int GetValue(int x, int y, int channel)
        {
            x = (int)(x / ScaleFactor);
            y = (int)(y / ScaleFactor);

            Debug.Assert(x >= 0 && x < Source.PixelWidth);
            Debug.Assert(y >= 0 && y < Source.PixelHeight);
            Debug.Assert(channel >= 0 && channel < 4);

            unsafe
            {
                byte* p = (byte*)Source.BackBuffer.ToPointer();
                int width = Source.PixelWidth;
                int height = Source.PixelHeight;
                int step = Source.BackBufferStride;

                return *(p + y * step + x * 4 + channel);
            }
        }

        private void Image_RenderArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Image image = sender as Image;
            var delta = e.Delta;
            var position = e.GetPosition(image);

            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                LockRendering();

                bool increasing = delta > 0;

                Point viewportCenter = ViewportCenterDefault;

                SwitchScaleFactor(increasing);

                OffsetX = (int)(viewportCenter.X * ScaleFactor - RenderingAreaWidth / 2);
                OffsetY = (int)(viewportCenter.Y * ScaleFactor - RenderingAreaHeight / 2);

                UnlockRendering();
            }
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                OffsetX += delta;
            }
            else if (Keyboard.Modifiers == ModifierKeys.None)
            {
                OffsetY += delta;
            }
        }

        private Point GetRenderingPoint(Point MousePoint)
        {
            return new Point(OffsetX + MousePoint.X, OffsetY + MousePoint.Y);
        }

        private void SwitchScaleFactor(bool increasing)
        {
            double parcent = ScaleFactor * 100;

            if (parcent > 100)
            {
                parcent += increasing ? 100 : -100;
            }
            else if (parcent == 100)
            {
                parcent += increasing ? 100 : -25;
            }
            else if (parcent > 25)
            {
                parcent += increasing ? 25 : -25;
            }
            else if (parcent == 25)
            {
                parcent += increasing ? 25 : -5;
            }
            else if (parcent > 5)
            {
                parcent += increasing ? 5 : -5;
            }
            else if (parcent == 5)
            {
                parcent += increasing ? 5 : -2.5;
            }
            else if (parcent > 2.5)
            {
                parcent += increasing ? 2.5 : -2.5;
            }
            else if (parcent == 2.5)
            {
                parcent += increasing ? 2.5 : -0.5;
            }
            else if (parcent > 0.5)
            {
                parcent += increasing ? 0.5 : -0.5;
            }
            else if (parcent == 0.5)
            {
                parcent += increasing ? 0.5 : 0;
            }

            ScaleFactor = parcent / 100.0;
        }

        public Point ViewportCenterDefault
        {
            get { return new Point((ViewportRect.Right + ViewportRect.Left) / 2, (ViewportRect.Bottom + ViewportRect.Top) / 2); }
        }

        public Point ViewportCenterCurrent
        {
            get { return GetViewportCenter(0); }
        }

        public Point ViewportLeftTopDefault
        {
            get { return ViewportRect.TopLeft; }
        }

        public Point ViewportLeftTopCurrent
        {
            get { return GetViewportLeftTop(0); }
        }

        public Point ViewportBottomRightDefault
        {
            get { return ViewportRect.BottomRight; }
        }

        public Point ViewportBottomRightCurrent
        {
            get { return GetViewportBottomRight(0); }
        }

        private Point GetViewportCenter(int delta)
        {
            var newScaleFactor = ScaleFactor + delta;
            return new Point(ViewportCenterDefault.X * newScaleFactor, ViewportCenterDefault.Y * newScaleFactor);
        }

        private Point GetViewportLeftTop(int delta)
        {
            var newScaleFactor = ScaleFactor + delta;
            var center = GetViewportCenter(delta);
            return new Point(center.X - RenderingAreaWidth / 2, center.Y - RenderingAreaHeight / 2);
        }

        private Point GetViewportBottomRight(int delta)
        {
            var newScaleFactor = ScaleFactor + delta;
            var center = GetViewportCenter(delta);
            return new Point(center.X + RenderingAreaWidth / 2, center.Y + RenderingAreaHeight / 2);
        }

        private Point GetViewportCenterByScaleFactor(double scaleFactor)
        {
            return new Point(ViewportCenterDefault.X * scaleFactor, ViewportCenterDefault.Y * scaleFactor);
        }

        private Point GetViewportLeftTopByScaleFactor(double scaleFactor)
        {
            var center = GetViewportCenterByScaleFactor(scaleFactor);
            return new Point(center.X * scaleFactor - RenderingAreaWidth / 2, center.Y * scaleFactor - RenderingAreaHeight / 2);
        }

        private Point GetViewportBottomRightByScaleFactor(double scaleFactor)
        {
            var center = GetViewportCenterByScaleFactor(scaleFactor);
            return new Point(center.X * scaleFactor + RenderingAreaWidth / 2, center.Y * scaleFactor + RenderingAreaHeight / 2);
        }

        private void UniformButton_Click(object sender, RoutedEventArgs e)
        {
            Uniform();
        }

        private void Scale32(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 32.0;
        }

        private void Scale31(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 31.0;
        }

        private void Scale30(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 30.0;
        }

        private void Scale29(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 29.0;
        }

        private void Scale28(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 28.0;
        }

        private void Scale27(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 27.0;
        }

        private void Scale26(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 26.0;
        }

        private void Scale25(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 25.0;
        }

        private void Scale24(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 24.0;
        }

        private void Scale23(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 23.0;
        }

        private void Scale22(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 22.0;
        }

        private void Scale21(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 21.0;
        }

        private void Scale20(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 20.0;
        }

        private void Scale19(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 19.0;
        }

        private void Scale18(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 18.0;
        }

        private void Scale17(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 17.0;
        }

        private void Scale16(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 16.0;
        }

        private void Scale15(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 15.0;
        }

        private void Scale14(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 14.0;
        }

        private void Scale13(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 13.0;
        }

        private void Scale12(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 12.0;
        }

        private void Scale11(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 11.0;
        }

        private void Scale10(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 10.0;
        }

        private void Scale9(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 9.0;
        }

        private void Scale8(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 8.0;
        }

        private void Scale7(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 7.0;
        }

        private void Scale6(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 6.0;
        }

        private void Scale5(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 5.0;
        }

        private void Scale4(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 4.0;
        }

        private void Scale3(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 3.0;
        }

        private void Scale2(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 2.0;
        }

        private void Scale1(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 1.0;
        }

        private void Scale075(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.75;
        }

        private void Scale05(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.5;
        }

        private void Scale025(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.25;
        }

        private void Scale02(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.2;
        }

        private void Scale015(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.15;
        }

        private void Scale01(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.1;
        }

        private void Scale005(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.05;
        }

        private void Scale0025(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.025;
        }

        private void Scale002(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.02;
        }

        private void Scale0015(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.015;
        }

        private void Scale001(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.01;
        }

        private void Scale0005(object sender, RoutedEventArgs e)
        {
            ScaleFactor = 0.005;
        }

        private void SwitchInterpolationToNearestNeighbor(object sender, RoutedEventArgs e)
        {
            Interpolation = Imaging.Interpolation.NearestNeighbor;
        }

        private void SwitchInterpolationToBilinear(object sender, RoutedEventArgs e)
        {
            Interpolation = Imaging.Interpolation.Bilinear;
        }

        private void SwitchInterpolationToBicubic(object sender, RoutedEventArgs e)
        {
            Interpolation = Imaging.Interpolation.Bicubic;
        }

        private void MenuItem_NearestNeighbor_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            menuItem.IsChecked = Interpolation == Imaging.Interpolation.NearestNeighbor;
        }

        private void MenuItem_Bilinear_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            menuItem.IsChecked = Interpolation == Imaging.Interpolation.Bilinear;
        }

        private void MenuItem_Bicubic_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            menuItem.IsChecked = Interpolation == Imaging.Interpolation.Bicubic;
        }
    }
}
