// Copyright © 2015 dhq_boiler.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace libSevenTools.WPFControls
{
    /// <summary>
    /// Interaction logic for DecoratedSlider.xaml
    /// </summary>
    public partial class DecoratedSlider : UserControl
    {
        #region DependencyProperty

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title",
            typeof(string),
            typeof(DecoratedSlider),
            new FrameworkPropertyMetadata("Title", new PropertyChangedCallback(OnTitleChanged)));

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum",
            typeof(double),
            typeof(DecoratedSlider),
            new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnMinimumChanged)));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum",
            typeof(double),
            typeof(DecoratedSlider),
            new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnMaximumChanged)));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(double),
            typeof(DecoratedSlider),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DecoratedSlider.OnValueChanged)));

        public static readonly DependencyProperty StepProperty = DependencyProperty.Register("Step",
            typeof(double),
            typeof(DecoratedSlider),
            new FrameworkPropertyMetadata(1d, new PropertyChangedCallback(OnStepChanged)));

        public static readonly DependencyProperty AutoProperty = DependencyProperty.Register("Auto",
            typeof(bool),
            typeof(DecoratedSlider),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DecoratedSlider.OnAutoChanged)));

        public static readonly DependencyProperty CanAutoProperty = DependencyProperty.Register("CanAuto",
            typeof(bool),
            typeof(DecoratedSlider),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(DecoratedSlider.OnCanAutoChanged)));

        public static readonly DependencyProperty IsEnableProperty = DependencyProperty.Register("IsEnable",
            typeof(bool),
            typeof(DecoratedSlider),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(DecoratedSlider.OnIsEnableChanged)));

        #endregion DependencyProperty

        #region DependencyPropertyChanged Callback

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DecoratedSlider ctrl = d as DecoratedSlider;
            if (ctrl != null)
            {
                ctrl.Label_TitlePresentedValue.Content = ctrl.Title;
            }
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecoratedSlider ctrl = d as DecoratedSlider;
            if (ctrl != null)
            {
                ctrl.Slider_PresentedValue.Minimum = ctrl.Minimum;
            }
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecoratedSlider ctrl = d as DecoratedSlider;
            if (ctrl != null)
            {
                ctrl.Slider_PresentedValue.Maximum = ctrl.Maximum;
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DecoratedSlider ctrl = d as DecoratedSlider;
            if (ctrl != null)
            {
                var e = new RoutedPropertyChangedEventArgs<double>((double)args.OldValue, (double)args.NewValue, ValueChangedEvent);
                ctrl.OnValueChanged(e);
            }
        }

        private static void OnStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecoratedSlider ctrl = d as DecoratedSlider;
            if (ctrl != null)
            {
                ctrl.Slider_PresentedValue.TickFrequency = ctrl.Step;
            }
        }

        private static void OnAutoChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DecoratedSlider ctrl = d as DecoratedSlider;
            if (ctrl != null)
            {
                var e = new RoutedPropertyChangedEventArgs<bool>((bool)args.OldValue, (bool)args.NewValue, AutoChangedEvent);
                ctrl.OnAutoChanged(e);
                ctrl.Slider_PresentedValue.IsEnabled = !(bool)args.NewValue;
            }
        }

        private static void OnCanAutoChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            DecoratedSlider ctrl = d as DecoratedSlider;
            if (ctrl != null)
            {
                var e = new RoutedPropertyChangedEventArgs<bool>((bool)args.OldValue, (bool)args.NewValue, CanAutoChangedEvent);
                ctrl.OnCanAutoChanged(e);
            }
        }

        private static void OnIsEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecoratedSlider ctrl = d as DecoratedSlider;
            if (ctrl != null)
            {
                if (ctrl.IsEnable)
                {
                    ctrl.Slider_PresentedValue.IsEnabled = true;
                    ctrl.SetBinding();
                }
                else
                {
                    ctrl.ToggleButton_Auto.IsEnabled = false;
                    ctrl.Button_Update.IsEnabled = false;
                    ctrl.Slider_PresentedValue.IsEnabled = false;
                    ctrl.Label_Value.Content = "N/A";
                    ctrl.Label_Minimum.Content = "N/A";
                    ctrl.Label_Maximum.Content = "N/A";
                }
            }
        }

        #endregion DependencyProprertyChanged Callback

        #region RoutedEvent

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventArgs<double>), typeof(DecoratedSlider));

        public static readonly RoutedEvent AutoChangedEvent = EventManager.RegisterRoutedEvent("AutoChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventArgs<bool>), typeof(DecoratedSlider));

        public static readonly RoutedEvent CanAutoChangedEvent = EventManager.RegisterRoutedEvent("CanAutoChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventArgs<bool>), typeof(DecoratedSlider));

        #endregion RoutedEvent

        #region RoutedPropertyChanged Callback

        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<double> args)
        {
            RaiseEvent(args);
        }

        protected virtual void OnAutoChanged(RoutedPropertyChangedEventArgs<bool> args)
        {
            RaiseEvent(args);
        }

        protected virtual void OnCanAutoChanged(RoutedPropertyChangedEventArgs<bool> args)
        {
            RaiseEvent(args);
        }

        #endregion RoutedPropertyChanged Callback

        #region Constructor

        public DecoratedSlider()
        {
            InitializeComponent();

            SetBinding();
        }

        #endregion Constructor

        private void SetBinding()
        {
            Binding labelBinding = new Binding("Value");
            labelBinding.Source = this;
            Label_Value.SetBinding(ContentProperty, labelBinding);

            Binding sliderBinding = new Binding("Value");
            sliderBinding.Source = this;
            Slider_PresentedValue.SetBinding(Slider.ValueProperty, sliderBinding);

            Binding toggleBinding = new Binding("CanAuto");
            toggleBinding.Source = this;
            ToggleButton_Auto.SetBinding(FrameworkElement.IsEnabledProperty, toggleBinding);

            Binding ToggleAutoBinding = new Binding("Auto");
            ToggleAutoBinding.Source = this;
            ToggleButton_Auto.SetBinding(ToggleButton.IsCheckedProperty, ToggleAutoBinding);

            Binding RefreshBinding = new Binding("Auto");
            RefreshBinding.Source = this;
            Button_Update.SetBinding(Button.IsEnabledProperty, RefreshBinding);

            Binding MinimumBinding = new Binding("Minimum");
            MinimumBinding.Source = this;
            Label_Minimum.SetBinding(Label.ContentProperty, MinimumBinding);

            Binding MaximumBinding = new Binding("Maximum");
            MaximumBinding.Source = this;
            Label_Maximum.SetBinding(Label.ContentProperty, MaximumBinding);
        }

        #region CLR DependencyProprety

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public bool Auto
        {
            get { return (bool)GetValue(AutoProperty); }
            set { SetValue(AutoProperty, value); }
        }

        public bool CanAuto
        {
            get { return (bool)GetValue(CanAutoProperty); }
            set { SetValue(CanAutoProperty, value); }
        }

        public bool IsEnable
        {
            get { return (bool)GetValue(IsEnableProperty); }
            set { SetValue(IsEnableProperty, value); }
        }

        #endregion CLR DependencyProperty

        #region CLR RoutedEvent Property

        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<bool> AutoChanged
        {
            add { AddHandler(AutoChangedEvent, value); }
            remove { RemoveHandler(AutoChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<bool> CanAutoChanged
        {
            add { AddHandler(CanAutoChangedEvent, value); }
            remove { RemoveHandler(CanAutoChangedEvent, value); }
        }

        #endregion CLR RoutedEvent

        #region CLR Event

        public event EventHandler ClickedUpdateButton;

        #endregion CLR Event

        #region CLR Event Callback

        protected virtual void OnClickedUpdateButton(EventArgs e)
        {
            if (ClickedUpdateButton != null)
            {
                ClickedUpdateButton(this, e);
            }
        }

        #endregion CLR Event Callback

        #region RoutedEvent Callback

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            OnClickedUpdateButton(new EventArgs());
        }

        #endregion RoutedEvent Callback
    }
}
