using System;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel.Commands
{
    /// <summary>
    /// General command type
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;         // lambda expression that executes the activity
        private readonly Func<object, bool>? _canExecute; // lambda expression that checks the condition of the activity

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Constructor of the DelegateCommand
        /// </summary>
        /// <param name="execute">Activity to execute</param>
        /// <param name="canExecute">Condition of enforceability</param>
        /// <exception cref="ArgumentNullException"></exception>
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

