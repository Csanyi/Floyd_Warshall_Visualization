using Floyd_Warshall.ViewModel.GraphComponents;
using System.ComponentModel;
using Floyd_Warshall_Model.Model;

namespace Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands
{
	/// <summary>
	/// Type of the canvas click command
	/// </summary>
	public class CanvasClickCommand : GraphCanvasCommandBase
    {
        private readonly GraphModel _graphModel;

        public CanvasClickCommand(GraphCanvasViewModel viewModel, GraphModel graphModel) : base(viewModel)
        {
            _graphModel = graphModel;
        }

        public override void Execute(object? parameter)
        {
            _graphModel.AddVertex();
        }

        public override bool CanExecute(object? parameter)
        {
            return !_viewModel.MaxVertexCountReached && base.CanExecute(parameter);
        }

        protected override void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(GraphCanvasViewModel.MaxVertexCountReached))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
