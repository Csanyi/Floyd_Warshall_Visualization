using Floyd_Warshall.ViewModel.GraphComponents;
using Floyd_Warshall_Model.Model;
using System;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.GraphCanvasCommands
{
	/// <summary>
	/// Type of the submit command
	/// </summary>
	public class SubmitCommand : GraphCanvasCommandBase
    {
        private readonly GraphModel _graphModel;

        public SubmitCommand(GraphCanvasViewModel viewModel, GraphModel graphModel) : base(viewModel)
        {
            _graphModel = graphModel;
        }

        public override void Execute(object? parameter)
        {
            try
            {
                short weight = Convert.ToInt16(_viewModel.WeightText);

                if (_viewModel.SelectedEdge != null)
                {
                    _graphModel.UpdateWeight(_viewModel.SelectedEdge.From.Id, _viewModel.SelectedEdge.To.Id, weight);
                }

                _viewModel.HasInputError = false;
            }
            catch(Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                _viewModel.HasInputError = true;
                if(_viewModel.SelectedEdge != null)
                {
                    _viewModel.WeightText = _viewModel.SelectedEdge.Weight.ToString();
                }
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return _viewModel.IsEdgeSelected && base.CanExecute(parameter);
        }

        protected override void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GraphCanvasViewModel.IsEdgeSelected))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
