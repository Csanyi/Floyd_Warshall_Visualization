using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.AlgorithmCommands
{
	/// <summary>
	/// Type of the algorithm start command
	/// </summary>
	public class AlgorithmStartCommand : CommandBase
    {
        private readonly AlgorithmViewModel _viewModel;

        public AlgorithmStartCommand(AlgorithmViewModel viewModel)
        {
            _viewModel = viewModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void Execute(object? parameter)
        {
            _viewModel.Timer.Start();
            _viewModel.IsStopped = false;
        }

        public override bool CanExecute(object? parameter)
        {
            return _viewModel.IsStopped && _viewModel.HasNextStep && _viewModel.IsInitialized;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AlgorithmViewModel.IsInitialized) || e.PropertyName == nameof(AlgorithmViewModel.IsStopped)
                 || e.PropertyName == nameof(AlgorithmViewModel.HasNextStep))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
