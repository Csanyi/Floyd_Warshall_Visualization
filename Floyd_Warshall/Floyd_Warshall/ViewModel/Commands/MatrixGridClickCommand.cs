using Floyd_Warshall_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall.ViewModel.Commands
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
            return _vm.IsInitialized && _vm.IsStopped && !_vm.IsNegCycleFound;
        }
    }
}
