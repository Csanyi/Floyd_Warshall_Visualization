using Floyd_Warshall_Model.Model;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.AlgorithmCommands
{
	/// <summary>
	/// Type of the algorithm cancel command
	/// </summary>
	public class AlgorithmCancelCommand : CommandBase
    {
        private readonly AlgorithmViewModel _viewModel;
        private readonly GraphModel _graphModel;

        public AlgorithmCancelCommand(AlgorithmViewModel viewModel, GraphModel graphModel)
        {
            _viewModel = viewModel;
            _graphModel = graphModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void Execute(object? parameter)
        {
            _viewModel.Timer.Stop();
            _graphModel.CancelAlgorithm();

            _viewModel.CallPropertyChanged(nameof(AlgorithmViewModel.IsInitialized));
            _viewModel.IsStopped = true;

            _viewModel.IsNegCycleFound = false;

            _viewModel.D.Clear();
            _viewModel.Pi.Clear();
            _viewModel.PrevD.Clear();
            _viewModel.PrevPi.Clear();
            _viewModel.VertexIds.Clear();
        }

        public override bool CanExecute(object? parameter)
        {
            return _viewModel.IsInitialized;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AlgorithmViewModel.IsInitialized))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
