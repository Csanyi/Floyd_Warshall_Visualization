using System;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<object> execute, Func<object, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter!);
        }

        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled");
            }
            _execute(parameter!);
        }
    }
}

