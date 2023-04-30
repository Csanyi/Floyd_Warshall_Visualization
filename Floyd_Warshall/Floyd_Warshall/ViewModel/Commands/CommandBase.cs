using System;
using System.Windows.Input;

namespace Floyd_Warshall.ViewModel.Commands
{
    /// <summary>
    ///  Type of the command base class
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        public abstract void Execute(object? parameter);

        /// <summary>
        /// Triggers the CanExecuteChanged event
        /// </summary>
        protected void OnCanExecuteChanged() 
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
