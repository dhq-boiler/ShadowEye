

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShadowEye.ViewModel
{
    public class CommandSinkBinding : System.Windows.Input.CommandBinding
    {
        private class CommonElement
        {
            private readonly FrameworkElement _fe;
            private readonly FrameworkContentElement _fce;
            public readonly bool IsValid;
            public event System.Windows.RoutedEventHandler Loaded
            {
                add
                {
                    this.Verify();
                    if (_fe != null)
                    {
                        _fe.Loaded += value;
                    }
                    else
                    {
                        _fce.Loaded += value;
                    }
                }
                remove
                {
                    this.Verify();
                    if (_fe != null)
                    {
                        _fe.Loaded -= value;
                    }
                    else
                    {
                        _fce.Loaded -= value;
                    }
                }
            }
            public System.Windows.Input.CommandBindingCollection CommandBindings
            {
                get
                {
                    this.Verify();
                    if (_fe != null)
                    {
                        return _fe.CommandBindings;
                    }
                    return _fce.CommandBindings;
                }
            }
            public bool IsLoaded
            {
                get
                {
                    this.Verify();
                    if (_fe != null)
                    {
                        return _fe.IsLoaded;
                    }
                    return _fce.IsLoaded;
                }
            }
            public CommonElement(DependencyObject depObj)
            {
                _fe = depObj as FrameworkElement;
                _fce = depObj as FrameworkContentElement;
                this.IsValid = (_fe != null || _fce != null);
            }
            private void Verify()
            {
                if (!this.IsValid)
                {
                    throw new System.InvalidOperationException("Cannot use an invalid CommmonElement.");
                }
            }
        }
        private ICommandSink _commandSink;
        public static readonly DependencyProperty CommandSinkProperty = DependencyProperty.RegisterAttached("CommandSink", typeof(ICommandSink), typeof(CommandSinkBinding), new UIPropertyMetadata(null, new PropertyChangedCallback(CommandSinkBinding.OnCommandSinkChanged)));
        public ICommandSink CommandSink
        {
            get
            {
                return _commandSink;
            }
            set
            {
                if (value == null)
                {
                    throw new System.ArgumentNullException("Cannot set CommandSink to null.");
                }
                if (_commandSink != null)
                {
                    throw new System.InvalidOperationException("Cannot set CommandSink more than once.");
                }
                _commandSink = value;
                base.CanExecute += delegate (object s, System.Windows.Input.CanExecuteRoutedEventArgs e)
                {
                    bool handled;
                    e.CanExecute = _commandSink.CanExecuteCommand(e.Command, e.Parameter, out handled);
                    e.Handled = handled;
                };
                base.Executed += delegate (object s, System.Windows.Input.ExecutedRoutedEventArgs e)
                {
                    bool handled;
                    _commandSink.ExecuteCommand(e.Command, e.Parameter, out handled);
                    e.Handled = handled;
                };
            }
        }
        public static ICommandSink GetCommandSink(DependencyObject obj)
        {
            return (ICommandSink)obj.GetValue(CommandSinkBinding.CommandSinkProperty);
        }
        public static void SetCommandSink(DependencyObject obj, ICommandSink value)
        {
            obj.SetValue(CommandSinkBinding.CommandSinkProperty, value);
        }
        private static void OnCommandSinkChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            ICommandSink commandSink = e.NewValue as ICommandSink;
            if (!CommandSinkBinding.ConfigureDelayedProcessing(depObj, commandSink))
            {
                CommandSinkBinding.ProcessCommandSinkChanged(depObj, commandSink);
            }
        }
        private static bool ConfigureDelayedProcessing(DependencyObject depObj, ICommandSink commandSink)
        {
            bool result = false;
            CommandSinkBinding.CommonElement elem = new CommandSinkBinding.CommonElement(depObj);
            if (elem.IsValid && !elem.IsLoaded)
            {
                System.Windows.RoutedEventHandler handler = null;
                handler = delegate (object param0, System.Windows.RoutedEventArgs param1)
                {
                    elem.Loaded -= handler;
                    CommandSinkBinding.ProcessCommandSinkChanged(depObj, commandSink);
                };
                elem.Loaded += handler;
                result = true;
            }
            return result;
        }
        private static void ProcessCommandSinkChanged(DependencyObject depObj, ICommandSink commandSink)
        {
            System.Windows.Input.CommandBindingCollection commandBindings = CommandSinkBinding.GetCommandBindings(depObj);
            if (commandBindings == null)
            {
                throw new System.ArgumentException("The CommandSinkBinding.CommandSink attached property was set on an element that does not support CommandBindings.");
            }
            foreach (System.Windows.Input.CommandBinding commandBinding in commandBindings)
            {
                CommandSinkBinding commandSinkBinding = commandBinding as CommandSinkBinding;
                if (commandSinkBinding != null && commandSinkBinding.CommandSink == null)
                {
                    commandSinkBinding.CommandSink = commandSink;
                }
            }
        }
        private static System.Windows.Input.CommandBindingCollection GetCommandBindings(DependencyObject depObj)
        {
            CommandSinkBinding.CommonElement commonElement = new CommandSinkBinding.CommonElement(depObj);
            if (!commonElement.IsValid)
            {
                return null;
            }
            return commonElement.CommandBindings;
        }
    }
}
