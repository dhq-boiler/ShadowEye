

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ShadowEye.Utils;

namespace ShadowEye.ViewModel
{
    public class CommandSink : Notifier, ICommandSink
    {
        private struct CommandCallbacks
        {
            public readonly Predicate<object> CanExecute;
            public readonly Action<object> Execute;
            public CommandCallbacks(Predicate<object> canExecute, Action<object> execute)
            {
                this.CanExecute = canExecute;
                this.Execute = execute;
            }
        }

        private readonly Dictionary<ICommand, CommandSink.CommandCallbacks> _commandToCallbacksMap = new Dictionary<ICommand, CommandCallbacks>();

        public void RegisterCommand(ICommand command, Predicate<object> canExecute, Action<object> execute)
        {
            CommandSink.VerifyArgument(command, "command");
            CommandSink.VerifyArgument(canExecute, "canExecute");
            CommandSink.VerifyArgument(execute, "execute");
            _commandToCallbacksMap[command] = new CommandSink.CommandCallbacks(canExecute, execute);
        }

        public void UnregisterCommand(ICommand command)
        {
            CommandSink.VerifyArgument(command, "command");
            if (_commandToCallbacksMap.ContainsKey(command))
            {
                _commandToCallbacksMap.Remove(command);
            }
        }

        public virtual bool CanExecuteCommand(System.Windows.Input.ICommand command, object parameter, out bool handled)
        {
            CommandSink.VerifyArgument(command, "command");
            if (_commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                return _commandToCallbacksMap[command].CanExecute(parameter);
            }
            return handled = false;
        }

        public virtual void ExecuteCommand(System.Windows.Input.ICommand command, object parameter, out bool handled)
        {
            CommandSink.VerifyArgument(command, "command");
            if (_commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                _commandToCallbacksMap[command].Execute(parameter);
                return;
            }
            handled = false;
        }

        private static void VerifyArgument(object arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }
    }
}
