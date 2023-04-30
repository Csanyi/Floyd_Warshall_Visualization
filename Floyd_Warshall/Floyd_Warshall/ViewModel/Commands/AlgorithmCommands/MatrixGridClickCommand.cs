using Floyd_Warshall_Model.Model;
using System;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.AlgorithmCommands
{
    /// <summary>
    /// Type of the matrix grid click command
    /// </summary>
    public class MatrixGridClickCommand : CommandBase
    {
        private readonly AlgorithmViewModel _viewModel;
        private readonly GraphModel _graphModel;

        public MatrixGridClickCommand(AlgorithmViewModel viewModel, GraphModel graphModel)
        {
            _viewModel = viewModel;
            _graphModel = graphModel;
        }

        public override void Execute(object? parameter)
        {
            if(parameter == null) { return; }

            Tuple<int, bool> values = (Tuple<int, bool>)parameter;

            int ind = Convert.ToInt32(values.Item1);
            bool isPrev = Convert.ToBoolean(values.Item2);

            MatrixGridViewModel grid = _viewModel.Pi[ind];

            _graphModel.GetRoute(grid.X, grid.Y, isPrev);
        }

        public override bool CanExecute(object? parameter)
        {
            return _viewModel.IsStopped && !_viewModel.IsNegCycleFound && _viewModel.IsInitialized;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AlgorithmViewModel.IsInitialized) || e.PropertyName == nameof(AlgorithmViewModel.IsStopped)
                    || e.PropertyName == nameof(AlgorithmViewModel.IsNegCycleFound))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
