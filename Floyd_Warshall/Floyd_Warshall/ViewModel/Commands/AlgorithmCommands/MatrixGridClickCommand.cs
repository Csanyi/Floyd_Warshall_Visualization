using Floyd_Warshall_Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int ind = Convert.ToInt32(parameter);

            MatrixGridViewModel grid = _vm.Pi[ind];

            _graphModel.GetRoute(grid.X, grid.Y);
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
