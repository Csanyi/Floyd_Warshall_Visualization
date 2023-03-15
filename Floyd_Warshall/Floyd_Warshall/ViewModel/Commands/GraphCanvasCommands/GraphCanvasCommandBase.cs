using Floyd_Warshall.ViewModel.GraphComponents;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands
{
    public abstract class GraphCanvasCommandBase : CommandBase
    {
        protected readonly GraphCanvasViewModel _viewModel;

        protected GraphCanvasCommandBase(GraphCanvasViewModel viewModel)
        { 
            _viewModel = viewModel;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }


        public override bool CanExecute(object? parameter)
        {
            return _viewModel.CanvasEnabled;
        }

        protected virtual void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(GraphCanvasViewModel.CanvasEnabled))
            {
                OnCanExecuteChanged();
            }
        }

    }
}
