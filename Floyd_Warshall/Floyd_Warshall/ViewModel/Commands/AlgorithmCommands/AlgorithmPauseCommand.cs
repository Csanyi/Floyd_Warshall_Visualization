using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.AlgorithmCommands
{
    /// <summary>
    /// Type of the algorithm pause command
    /// </summary>
    public class AlgorithmPauseCommand : CommandBase
    {
        private readonly AlgorithmViewModel _viewModel;

        public AlgorithmPauseCommand(AlgorithmViewModel viewModel)
        {
            _viewModel = viewModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void Execute(object? parameter)
        {
            _viewModel.Timer.Stop();
            _viewModel.IsStopped = true;

            if(_viewModel.TimerInterval < AlgorithmViewModel.CriticalTime)
            {
                _viewModel.UpdateData();
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return _viewModel.HasNextStep && !_viewModel.IsStopped && _viewModel.IsInitialized;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AlgorithmViewModel.HasNextStep) || e.PropertyName == nameof(AlgorithmViewModel.IsStopped)
                 || e.PropertyName == nameof(AlgorithmViewModel.IsInitialized))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
