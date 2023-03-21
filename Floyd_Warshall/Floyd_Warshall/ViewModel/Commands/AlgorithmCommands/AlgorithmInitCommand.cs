using Floyd_Warshall_Model;
using System.ComponentModel;
using System.Windows;

namespace Floyd_Warshall.ViewModel.Commands.AlgorithmCommands
{
    public class AlgorithmInitCommand : CommandBase
    {
        private readonly AlgorithmViewModel _viewModel;
        private readonly GraphModel _graphModel;

        public AlgorithmInitCommand(AlgorithmViewModel viewModel, GraphModel graphModel)
        {
            _viewModel = viewModel;
            _graphModel = graphModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override void Execute(object? parameter)
        {
            _graphModel.GetVertexIds().ForEach(id => _viewModel.VertexIds.Add(id));

            _viewModel.Size = _viewModel.VertexIds.Count;

            _viewModel.CallPropertyChanged(nameof(AlgorithmViewModel.VertexIds));

            _graphModel.StartAlgorithm();

            _viewModel.CallPropertyChanged(nameof(AlgorithmViewModel.IsInitialized));
        }

        public override bool CanExecute(object? parameter)
        {
            return _viewModel.IsEnoughVerteces;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(AlgorithmViewModel.IsEnoughVerteces))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
