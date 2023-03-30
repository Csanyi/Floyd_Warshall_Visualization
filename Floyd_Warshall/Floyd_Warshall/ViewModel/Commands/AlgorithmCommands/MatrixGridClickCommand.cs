using Floyd_Warshall_Model.Model;
using System;
using System.ComponentModel;

namespace Floyd_Warshall.ViewModel.Commands.AlgorithmCommands
{
    public class MatrixGridClickCommand : CommandBase
    {
        private readonly AlgorithmViewModel _vm;
        private readonly GraphModel _graphModel;

        public MatrixGridClickCommand(AlgorithmViewModel vm, GraphModel graphModel)
        {
            _vm = vm;
            _graphModel = graphModel;
        }

        public override void Execute(object? parameter)
        {
            if(parameter == null) { return; }

            Tuple<int, bool> values = (Tuple<int, bool>)parameter;

            int ind = Convert.ToInt32(values.Item1);
            bool isPrev = Convert.ToBoolean(values.Item2);

            MatrixGridViewModel grid = _vm.Pi[ind];

            _graphModel.GetRoute(grid.X, grid.Y, isPrev);
        }

        public override bool CanExecute(object? parameter)
        {
            return _vm.IsStopped && !_vm.IsNegCycleFound && _vm.IsInitialized;
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
